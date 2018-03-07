// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open EasyTypes.SqlGenerator
open DotNetParser
open System.IO


let createDir (folder:string) =
    if (not (Directory.Exists folder)) then
        Directory.CreateDirectory folder |> ignore

let parseArgs (argv:string array) =
    if (argv.Length > 0) then
        (argv.[0], argv.[1])
    else 
        (@".\Samples", @".\GeneratedSamples")

[<EntryPoint>]
let main argv = 
    let (inputFOlder, outputFolder) = parseArgs argv
    let semanticTypes, _ = SemanticCompiler.CompileFolder inputFOlder 
    semanticTypes 
        |> Seq.iter (fun t ->
            let newFolder = (Path.Combine(outputFolder,"Tables"))
            createDir newFolder
            
            File.WriteAllText(newFolder + "\\" + t.name + ".sql", SqlTableGenerator.buildTable t |> SqlCommon.FormatSql)
        ) 
    semanticTypes 
        |> Seq.iter (fun t ->
            let newFolder = (Path.Combine(outputFolder,"StoredProcedures"))
            createDir newFolder
            File.WriteAllText(newFolder + "\\spInsert" + t.name + ".sql", SqlStoredProcGenerator.buildInsertSp t |> SqlCommon.FormatSql)
            File.WriteAllText(newFolder + "\\spGet" + t.name + ".sql", SqlStoredProcGenerator.buildGetSp t |> SqlCommon.FormatSql)
            File.WriteAllText(newFolder + "\\spUpdate" + t.name + ".sql", SqlStoredProcGenerator.buildUpdateSp t |> SqlCommon.FormatSql)
        ) 

    0 // return an integer exit code
