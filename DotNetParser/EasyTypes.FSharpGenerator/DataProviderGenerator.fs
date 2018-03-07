namespace EasyTypes.FSharpGenerator

module DataProviderGenerator = 
    open DotNetParser.SemanticTypes
    open System
    open System
    open System

    
    let isPrimaryKey  (p:TypeProperty) = 
        p.propType.name = "IntId"

    let private isAutoDateColumn (p:TypeProperty) =
        p.name = "CreatedOn" || p.name = "ModifiedOn" 
        
    let private autoGenColumn (p:TypeProperty) =
        isAutoDateColumn p || isPrimaryKey p

    let private buildParam (p:TypeProperty): string =
        let firstLetter = p.name.[0].ToString().ToLower()
        let rest = p.name.Substring(1)
        p.name + " = " + (firstLetter + rest)
    let private buildParamForInsert (p:TypeProperty): string =
        let firstLetter = p.name.[0].ToString().ToUpper()
        let rest = p.name.Substring(1)
        p.name + " = rendition." + (firstLetter + rest)

    let private buildParamForGetResult (p:TypeProperty): string =
        let firstLetter = p.name.[0].ToString().ToUpper()
        let rest = p.name.Substring(1)
        (firstLetter + rest) + " = row." + p.name 

    let private buildGetParam (p:TypeProperty): string =
        let firstLetter = p.name.[0].ToString().ToLower()
        let rest = p.name.Substring(1)
        "(" + (firstLetter + rest) +  " : int)"
        
    let GenerateGet (customType: CustomType ) : string =
        let idCols = customType.props |> Seq.filter isPrimaryKey  |> Seq.map buildGetParam
        let passedParams = customType.props |> Seq.filter isPrimaryKey |> Seq.map buildParam
        let paramsForReturnRecord = customType.props |> Seq.map buildParamForGetResult
        let funcName = "Get" + customType.name
        let renditionType = customType.name + "Rendition"
        let paramsTxt = if idCols |> Seq.isEmpty then "()" else (String.concat " " idCols) 
        "

    let " + funcName + " " + paramsTxt + " =
       asyncTrial {
            use queryCmd = new DbSchema.dbo.sp" + funcName + "ById()
            try
                let! sqlresults = queryCmd.AsyncExecuteSingle(" + (String.concat ", " passedParams) + ")
                return 
                    match sqlresults with
                    | Some row -> 
                        Some  {
                            " + (String.concat "; " paramsForReturnRecord ) + "
                        }
                    |None -> None
                

            with
            | ex ->
                let! u = failWithException<_> ex \"" + funcName + "\" 
                return u
        }
        "
    
    let private buildIdAssignment (p:TypeProperty): string =
        let firstLetter = p.name.[0].ToString().ToUpper()
        let rest = p.name.Substring(1)
        (firstLetter + rest) +  " = idOrDefault idGenerated"
        
    let GenerateUpdate (customType: CustomType ) : string =
        let passedParams = customType.props |> Seq.filter (isAutoDateColumn >> not) |> Seq.map buildParamForInsert
        let funcName = "Update" + customType.name
        "

    let " + funcName + " (rendition: " + customType.name + "Rendition ) =
        asyncTrial {
            use cmd = new DbSchema.dbo.spUpdate" + (customType.name) + "()
            try
                let! _ = cmd.AsyncExecute(" + (String.concat ", " passedParams) + ")
                return rendition
            with
            |  ex ->
                let! u = failWithException<_> ex \"" + funcName + "\" 
                return u
        }
        "
    let GenerateAdd nameSpace (customType: CustomType ) : string =
        let idAssignments = customType.props |> Seq.filter isPrimaryKey  |> Seq.map buildIdAssignment
        let passedParams = customType.props |> Seq.filter (autoGenColumn >> not) |> Seq.map buildParamForInsert
        let moduleName =  customType.name + "Data" 
        let funcName = "Add" + customType.name
        let withCode = if idAssignments |> Seq.isEmpty then "rendition" else "{ rendition with " + (String.concat "; " idAssignments) + " }"
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
                let! idGenerated = cmd.AsyncExecuteSingle(" + (String.concat ", " passedParams) + ")
                return " + withCode + "
            with
            |  ex ->
                let! u = failWithException<_> ex \"" + funcName + "\" 
                return u
        }
    " + (GenerateGet customType) + "
    " + (GenerateUpdate customType) + "
        "
