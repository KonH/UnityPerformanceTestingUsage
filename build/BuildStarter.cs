using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using System.Diagnostics;

[UnsetVisualStudioEnvironmentVariables]
class BuildStarter : NukeBuild {
	[Parameter]
	public string UnityPath;

	public static int Main() => Execute<BuildStarter>(x => x.Test);

	Target Test => _ => _
		.Requires(() => UnityPath)
		.Executes(() => {
			ProcessTasks.StartProcess(UnityPath, "-runTests -automated -projectPath . -testResults TestResults.xml");
		});

	Target BuildReporter => _ => _
		.Executes(() => {
			DotNetTasks.DotNetBuild(new DotNetBuildSettings()
				.SetProjectFile("PerformanceBenchmarkReporter/UnityPerformanceBenchmarkReporter")
				.SetConfiguration("Release"));
		});

	Target CreateReport => _ => _
		.DependsOn(BuildReporter)
		.Executes(() => {
			using var proc = Process.Start(
				"PerformanceBenchmarkReporter/UnityPerformanceBenchmarkReporter/bin/Release/netcoreapp3.0/UnityPerformanceBenchmarkReporter.exe",
				"--baseline=TestResults.xml --results=TestResults.xml --reportdirpath=.");
			proc.WaitForExit();
		});
}
