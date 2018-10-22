using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NetPack.Requirements
{
    public class NpmModuleRequirement : IRequirement
    {

        private Dictionary<string, List<string>> _installedPackages = null;

        private readonly string _packageName;
        private readonly string _version = null;
        private readonly bool _installIfNotFound = false;

        public NpmModuleRequirement(string packageName, bool installIfNotFound, string version = null)
        {
            _packageName = packageName;
            _version = version;
            _installIfNotFound = installIfNotFound;
        }

        public Dictionary<string, List<string>> GetInstalledPackages()
        {
            if (_installedPackages == null)
            {
                _installedPackages = DetectPackages();
            }

            return _installedPackages;
        }

        private Dictionary<string, List<string>> DetectPackages()
        {
            // query local npm modules, because requiring global modules within nodejs
            // requires an environment variable to be set: http://stackoverflow.com/questions/15636367/nodejs-require-a-global-module-package

            using (Process p = ProcessUtils.CreateNpmProcess("list --depth 0"))
            {
                p.Start();

                // make sure it finished executing before proceeding 
                p.WaitForExit();

                var errors = new List<string>();
                var warnings = new List<string>();
                // reads the error output
                while (!p.StandardError.EndOfStream)
                {
                    string line = p.StandardError.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.Contains("WARN"))
                        {
                            warnings.Add(line);
                            // WARNINGS ARE OK.
                        }
                        else if (line.StartsWith("npm ERR! extraneous"))
                        {
                            warnings.Add(line);
                        }
                        else
                        {
                            errors.Add(line);
                        }
                    }
                }

                if (errors.Any())
                {
                    string errorMessage = string.Join(Environment.NewLine, errors);
                    // if there were errors, throw an exception
                    if (!String.IsNullOrEmpty(errorMessage))
                    {
                        throw new NodeJsNotInstalledException(errorMessage);
                    }
                }

                string output = p.StandardOutput.ReadToEnd();
                return ParsePackagesFromStdOut(output);

            }

        }

        private Dictionary<string, List<string>> ParsePackagesFromStdOut(string output)
        {

            Dictionary<string, List<string>> packages = new Dictionary<string, List<string>>();

            string asciiEncoded = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(output));

            StringReader reader = new StringReader(asciiEncoded);
            string line = reader.ReadLine(); // first line is environment current directory.

            char[] nonAsciiChars = new char[] { '?' }; // chars that could not be ascii encoded are turned into ? so we remove them.

            while (reader.Peek() != -1)
            {
                string packageLine = reader.ReadLine();
                if (packageLine.StartsWith("?") || packageLine.StartsWith("+--"))
                {
                    //  var lastIndex = packageLine.LastIndexOf('?');
                    //  var packageName = packageLine.Substring(lastIndex + 1);
                    string packageNameWithVersion = packageLine.TrimStart(nonAsciiChars).Trim();
                    packageNameWithVersion = packageNameWithVersion.TrimStart(new char[] { '+', '-', '-' }).Trim();

                    if (packageNameWithVersion == "(empty)")
                    {
                        continue;
                    }

                    string packageName;
                    string packageVersion;

                    int indexOfVersionSeperator = packageNameWithVersion.IndexOf('@');
                    if (indexOfVersionSeperator != -1)
                    {
                        packageVersion = packageNameWithVersion.Substring(indexOfVersionSeperator + 1);
                        packageName = packageNameWithVersion.Substring(0, indexOfVersionSeperator);
                    }
                    else
                    {
                        packageVersion = "";
                        packageName = packageNameWithVersion;
                    }

                    if (!packages.ContainsKey(packageName))
                    {
                        packages.Add(packageName, new List<string>());
                    }

                    List<string> versionsList = packages[packageName];
                    if (!versionsList.Contains(packageVersion))
                    {
                        packages[packageName].Add(packageVersion);
                    }
                }
            }

            return packages;

        }

        public virtual void Check()
        {
            Dictionary<string, List<string>> installedPackages = GetInstalledPackages();

            if (!installedPackages.ContainsKey(_packageName))
            {
                if (_installIfNotFound)
                {
                    // try and install the requird npm module.
                    InstallNpmPackage(_packageName, _version);
                    return;
                }
            }

            // package already installed with same name, but is it the required version? 
            if (_version != null)
            {
                // TODO: Support min version or semantic version range
                List<string> installedPackageVersions = installedPackages[_packageName];
                if (!installedPackageVersions.Contains(_version))
                {
                    InstallNpmPackage(_packageName, _version);
                }
            }

        }

        private void InstallNpmPackage(string packageName, string version = null)
        {
            string args = string.IsNullOrWhiteSpace(version)
                ? $"install {packageName}"
                : $"install {packageName}@{version}";

            using (Process p = ProcessUtils.CreateNpmProcess(args))
            {
                p.Start();

                // make sure it finished executing before proceeding 
                p.WaitForExit();

                // reads the error output
                List<string> errors = new List<string>();
                List<string> warnings = new List<string>();

                while (!p.StandardError.EndOfStream)
                {
                    string line = p.StandardError.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.Contains("WARN"))
                        {
                            warnings.Add(line);
                            // WARNINGS ARE OK.
                        }
                        else if (line.StartsWith("npm notice"))
                        {
                            warnings.Add(line);
                        }                       
                        else
                        {
                            errors.Add(line);
                        }
                    }
                }

                if (errors.Any())
                {
                    string errorMessage = string.Join(Environment.NewLine, errors);
                    // if there were errors, throw an exception
                    if (!String.IsNullOrEmpty(errorMessage))
                    {
                        throw new NpmPackageCouldNotBeInstalledException(errorMessage);
                    }
                }




                string output = p.StandardOutput.ReadToEnd();


            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() == typeof(NpmModuleRequirement))
            {
                NpmModuleRequirement req = (NpmModuleRequirement)obj;
                return req._packageName == _packageName && req._version == _version;
            }
            return false;
        }

    }
}