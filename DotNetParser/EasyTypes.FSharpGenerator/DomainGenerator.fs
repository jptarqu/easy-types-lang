namespace EasyTypes.FSharpGenerator

module DomainGenerator =
    open DotNetParser.SemanticTypes
    open System

    let cleanDomainPropName (s:string) =
        let firstLetter = s.[0].ToString().ToUpper()
        let rest = s.Substring(1)
        (firstLetter + rest)
    let cleanVarName (s:string) =
        let firstLetter = s.[0].ToString().ToLower()
        let rest = s.Substring(1)
        (firstLetter + rest)

    let private buildDomainProp (p:TypeProperty): string =
        (cleanDomainPropName p.name) +  " : " + p.propType.name + ".T"
        
    let private buildDomainPropCreator (p:TypeProperty): string =
        "let " + (cleanDomainPropName p.name) +  " value = CommonValidations.ToPropResult \"" + (cleanDomainPropName p.name) + "\" (" + p.propType.name + ".create value) "

    let private buildDomainPropAsError (p:TypeProperty): string =
        "CommonValidations.AsError (Props." + (cleanDomainPropName p.name) +  " r." + (cleanDomainPropName p.name) +  ")"
    let private buildDomainPropAsAssignment (p:TypeProperty): string =
        "let! " + (cleanVarName p.name) +  " = Props." + (cleanDomainPropName p.name) +  " r." + (cleanDomainPropName p.name)
    let private buildDomainPropAsRecordPropAssignment (p:TypeProperty): string =
        (cleanDomainPropName p.name) +  " = " +  (cleanVarName p.name)

    let Generate nameSpace (customType: CustomType ) : string =
        let renditionType = customType.name + "Rendition"
        let moduleName = customType.name + "Domain"
        let props = customType.props |> Seq.map buildDomainProp
        let propsCreators = customType.props |> Seq.map buildDomainPropCreator
        let propsAsError = customType.props |> Seq.map buildDomainPropAsError
        let propsAssignments = customType.props |> Seq.map buildDomainPropAsAssignment
        let recordAssignments = customType.props |> Seq.map buildDomainPropAsRecordPropAssignment
        "namespace " + nameSpace + "

module " + moduleName + " =
    open Chessie
    open System
    open FsCommons.Core
    open FsCommons.Core.Chessie
    
    type D =  private {
        " + (String.concat "\n        " props) + "
    }
    type T = private " + moduleName + " of D

    module Props =
         " + (String.concat "\n         " propsCreators) + "
    
    let validate (r: " + renditionType + ")  =
        [
            " + (String.concat "\n            " propsAsError) + "
        ]
        |> CommonValidations.FailIfErros 
           
    let create (r: " + renditionType + ") : RopResult<T,_> =
        trial {
            let! _ = validate r
            " + (String.concat "\n            " propsAssignments) + "
            return " + moduleName + " {
                " + (String.concat "\n                " recordAssignments) + "
            }
        }
        "