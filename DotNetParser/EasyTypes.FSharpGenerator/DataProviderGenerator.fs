namespace EasyTypes.FSharpGenerator

module DataProviderGenerator = 
    open DotNetParser.SemanticTypes

    
    let private isAutoDateColumn (p:TypeProperty) =
        p.name = "CreatedOn" || p.name = "ModifiedOn" 

    let private buildParam (p:TypeProperty): string =
        p.name + " = rendition." + p.name

    let GenerateAdd nameSpace (customType: CustomType ) : string =
        let passedParams = customType.props |> Seq.filter (isAutoDateColumn >> not) |> Seq.map buildParam
        let moduleName =  customType.name + "Data" 
        let funcName = "Add" + customType.name
        "namespace " + nameSpace + "

module " + moduleName  + " = 
    open " + nameSpace + ".Connection
    open FsCommons.Core.Chessie
    open Chessie.ErrorHandling
    open FsCommons.Core
    open System

    let " + funcName + " (rendition: " + customType.name + "Rendition ) =
        asyncTrial {
            use cmd = new DbSchema.dbo.spInsert" + (customType.name) + "()
            try
                let! results = cmd.AsyncExecute(" + (String.concat ", " passedParams) + ")
                return 0
            with
            |  ex ->
                let! u = failWithException<int> ex \"" + funcName + "\" 
                return u
        }
        "
