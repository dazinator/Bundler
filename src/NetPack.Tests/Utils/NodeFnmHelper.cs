using System;

namespace NetPack.Tests.Utils;

public static class NodeFnmHelper
{
    public static void SetPath()
    {
        var currentPath = Environment.GetEnvironmentVariable("PATH");
        Environment.SetEnvironmentVariable("PATH",
            $"C:\\Users\\darre\\AppData\\Local\\fnm_multishells\\12124_1716904606883;{currentPath}");
    }
}
