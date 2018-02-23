namespace EasyTypes.FSharpGenerator

module RenditionGenerator =
    open DotNetParser.SemanticTypes
    open System

    let private buildRenditionProp (p:TypeProperty): string =
        let firstLetter = p.name.[0].ToString().ToUpper()
        let rest = p.name.Substring(1)
        (firstLetter + rest) +  " : " + p.propType.baseType.GetRenditionTypeName()
    let Generate nameSpace (customType: CustomType ) : string =
        let renditionType = customType.name + "Rendition"
        let props = customType.props |> Seq.map buildRenditionProp
        "namespace " + nameSpace + "
open System

[<CLIMutableAttribute>]
type " + renditionType + " = {
    " +
(String.concat "\n    " props) + "
}"