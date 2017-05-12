var NUGET_VERSION = Argument ("nugetversion", "0.0.0.1");

var TARGET = Argument ("target", "nuget");

Task ("libs").Does (() => {

    EnsureDirectoryExists ("./artifacts/");

    NuGetRestore ("./Plugin.SocialAuth.sln");

    MSBuild ("./Plugin.SocialAuth.sln", c => c.Configuration = "Release");
});

Task ("nuget").IsDependentOn("libs").Does (() => {
    var nuspecs = GetFiles ("./NuSpec/*.nuspec");

    foreach (var ns in nuspecs) {
        NuGetPack (ns, new NuGetPackSettings {
            Version = NUGET_VERSION,
            BasePath = "./NuSpec",
            OutputDirectory = "./artifacts/",
        });
    }
});

RunTarget (TARGET);