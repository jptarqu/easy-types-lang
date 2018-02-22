namespace EasyTypes.FSharpGenerator

module DataProviderGenerator = 
    open DotNetParser.SemanticTypes
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

    let private buildGetParam (p:TypeProperty): string =
        let firstLetter = p.name.[0].ToString().ToLower()
        let rest = p.name.Substring(1)
        "(" + (firstLetter + rest) +  " : int)"
        
    let GenerateGet (customType: CustomType ) : string =
        let idCols = customType.props |> Seq.filter isPrimaryKey  |> Seq.map buildGetParam
        let passedParams = customType.props |> Seq.filter isPrimaryKey |> Seq.map buildParam
        let paramsForReturnRecord = customType.props |> Seq.map buildParam
        let funcName = "Get" + customType.name
        let renditionType = customType.name + "Rendition"
        "

    let " + funcName + " " + (String.concat " " idCols) + " =
       asyncTrial {
            use queryCmd = new DbSchema.dbo.sp" + funcName + "ById()
            try
                let! sqlresults = queryCmd.AsyncExecuteSingle(" + (String.concat ", " passedParams) + ")
                return 
                    match sqlresults with
                    | Some rendition -> 
                        Some  {
                            " + (String.concat "; " paramsForReturnRecord ) + "
                        }
                    |None -> None
                

            with
            | ex ->
                let! u = failWithException<" + renditionType + " option> ex \"" + funcName + "\" 
                return u
        }
        "

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
    " + (GenerateGet customType) + "
        "
