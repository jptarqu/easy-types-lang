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
    let private buildPropAsRenditionAssignment (p:TypeProperty): string =
        (cleanDomainPropName p.name) +  " = " + p.propType.name + ".ToRendition d." +  (cleanDomainPropName p.name)

    let Generate nameSpace (customType: CustomType ) : string =
        let renditionType = customType.name + "Rendition"
        let moduleName = customType.name + "Domain"
        let props = customType.props |> Seq.map buildDomainProp
        let propsCreators = customType.props |> Seq.map buildDomainPropCreator
        let propsAsError = customType.props |> Seq.map buildDomainPropAsError
        let propsAssignments = customType.props |> Seq.map buildDomainPropAsAssignment
        let recordAssignments = customType.props |> Seq.map buildDomainPropAsRecordPropAssignment
        let renditionRecordAssignments = customType.props |> Seq.map buildPropAsRenditionAssignment
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
    
    let Validate (r: " + renditionType + ")  =
        [
            " + (String.concat "\n            " propsAsError) + "
        ]
        |> CommonValidations.FailIfErros 
           
    let Create (r: " + renditionType + ") : RopResult<T,_> =
        trial {
            let! _ = Validate r
            " + (String.concat "\n            " propsAssignments) + "
            return " + moduleName + " {
                " + (String.concat "\n                " recordAssignments) + "
            }
        }

    let Apply f (" + moduleName + " s) =
        f s

    let ToRendition (" + moduleName + " d) : " + renditionType + " =
        {
                " + (String.concat "\n                " renditionRecordAssignments) + "
        }
        "