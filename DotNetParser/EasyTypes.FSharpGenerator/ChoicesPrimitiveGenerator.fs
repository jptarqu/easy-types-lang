namespace EasyTypes.FSharpGenerator

module ChoicesPrimitiveGenerator =
    open DotNetParser.PrimitivesParserTypes

    let buildChoicesUnion (choices: IdLabelPair<string> array) = 
        choices 
        |> Array.map (fun (i, v) -> i)
    let buildDomainPairs (choices: IdLabelPair<string> array) = 
        choices 
        |> Array.map (fun (i, v) -> sprintf "(\"%s\", %s)" i i )
    let buildLabelPairs (choices: IdLabelPair<string> array) = 
        choices 
        |> Array.map (fun (i, v) -> sprintf "(\"%s\", \"%s\")" i v )
    let buildLabelDomainPairs (choices: IdLabelPair<string> array) = 
        choices 
        |> Array.map (fun (i, v) -> sprintf "(%s, \"%s\")" i v )

    let Generate (primitiveName: string) (req: CommonDataRequirementsStringChoices) = 
        let choicesUnion = req.Choices |> buildChoicesUnion 
        let domainPairs = req.Choices |> buildDomainPairs
        let labelPairs = req.Choices |> buildLabelPairs
        let  labelDomainPairs  = req.Choices |> buildLabelDomainPairs
        let baseType = "string"
        "
    
    type D = " +  (String.concat " | " choicesUnion) + "
    
    let IdDomainPairs = 
        [
            " +  (String.concat "\n            " domainPairs) + "
        ]
    let IdLabelPairs = 
        [
            " +  (String.concat "\n            " labelPairs) + "
        ]
    let DomainLabelPairs = 
        [
            " +  (String.concat "\n            " labelDomainPairs) + "
        ]

    type T = 
        private
        | " + primitiveName + " of D
    
    let Apply f (" + primitiveName + " s) = f s

    let Create(s : string) = s
                             |> CommonValidations.FromStringId IdDomainPairs
                             >=> " + primitiveName + "
    let FromString = Create
    let ToString(" + primitiveName + " s) : string = 
        let itemFound = IdDomainPairs |> Seq.tryFind (fun (id, value) -> value = s) 
        match itemFound with
        | None -> \"UNKOWN\"
        | Some  (id, value) -> id

    let ToRendition = ToString 
    
    let ToLabel(" + primitiveName + " s) : string = 
        let itemFound = DomainLabelPairs |> Seq.tryFind (fun (id, value) -> id = s) 
        match itemFound with
        | None -> \"UNKOWN\"
        | Some  (id, value) -> value
        "

