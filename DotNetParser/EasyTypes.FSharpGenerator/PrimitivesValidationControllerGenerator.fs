namespace EasyTypes.FSharpGenerator

module PrimitivesValidationControllerGenerator =
    open DotNetParser.SemanticTypes

    let private buildDictAdd (p: CustomPrimitive) =
        "fieldValidators.Add(\"" + p.name + "\", " + p.name + ".FromString >> CommonValidations.AsJsonStringValidationResult )"
    let Generate (allPrimitives: CustomPrimitive seq) =
        let dictAdditions = allPrimitives |> Seq.map buildDictAdd 
        "namespace FsCommons.Controllers

open Microsoft.AspNetCore.Mvc
open Chessie.ErrorHandling
open System.Collections.Generic
open FsCommons.Core
open Chessie

type PropertyValidatorController() =
    inherit Controller() 
    
    let typeFieldName = \"type\"
    let fieldValidators = new Dictionary<string, string -> string>()
    do
        " + (String.concat "\n        " dictAdditions) + " 

    [<HttpGet>]
    member this.Validate() =
        let lastValueKey = this.Request.Query.Keys |> Seq.filter (fun k -> k <> typeFieldName) |> Seq.head
        let lastValue = this.Request.Query.[lastValueKey].ToString()
        let fieldType = this.Request.Query.[typeFieldName].ToString()
        fieldValidators.[fieldType.ToLower()](lastValue)
        "

