import jetbrains.buildServer.configs.kotlin.*
import jetbrains.buildServer.configs.kotlin.buildSteps.*
import jetbrains.buildServer.configs.kotlin.triggers.vcs

/*
 * Generated 2026-09-08 as part of the Cloud migration (Platform onboarding).
 * Build and test only. Publish is added later, together with the versioning
 * decision and the feed write token - see VM.IaC docs/teamcity.md, the Platform flow section.
 * Restore resolves through the agent's machine config (GitHub Packages + nuget.org).
 */

version = "2026.1"

project {

    buildType {
        id("Build")
        name = "Build"

        vcs {
            root(DslContext.settingsRoot)
        }

        steps {
            dotnetRestore {
                name = "Restore"
                projects = "VM.Lab.Interfaces.BlobAnalyzer.sln"
            }
            dotnetBuild {
                name = "Build"
                projects = "VM.Lab.Interfaces.BlobAnalyzer.sln"
                configuration = "Release"
            }
            dotnetTest {
                name = "Test"
                projects = "VM.Lab.Interfaces.BlobAnalyzer.sln"
                configuration = "Release"
            }
        }

        triggers {
            vcs {
            }
        }

        requirements {
            equals("vmlab.role.build", "true")
        }
    }
}
