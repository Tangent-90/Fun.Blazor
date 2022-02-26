﻿namespace Fun.Blazor.Cli

open System.ComponentModel
open Spectre.Console.Cli
open Fun.Blazor.Generator


type WatchSettings() =
    inherit CommandSettings()

    [<Description("Project you want to watch")>]
    [<CommandArgument(0, "[PROJECT]")>]
    member val Project = "" with get, set

    [<Description("Server for receiving code changes")>]
    [<CommandOption("-s|--server")>]
    member val Server = "" with get, set


type CodeGenSettings() =
    inherit CommandSettings()

    [<Description("Project you want to add bindings")>]
    [<CommandArgument(0, "[PROJECT]")>]
    member val Project = "" with get, set

    [<Description("Output directory of generated codes")>]
    [<CommandOption("-o|--outDir")>]
    member val OutDir = "Fun.Blazor.Bindings" with get, set

    [<Description("Dsl style Feliz|CE")>]
    [<CommandOption("-s|--style")>]
    member val Style = Style.CE with get, set

    [<Description(".NET SDK version")>]
    [<CommandOption("--sdk")>]
    member val Sdk = "" with get, set
    
    [<Description("Fun.Blazor.Generator version")>]
    [<CommandOption("--generator-version")>]
    member val GeneratorVersion = "2.0.0-beta015" with get, set
    
    [<Description("Turn on inline option for generated code")>]
    [<CommandOption("--inline")>]
    [<DefaultValue true>] // Default value for bool is different
    member val Inline = true with get, set