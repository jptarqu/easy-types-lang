namespace DotNetParser

module CustomLookupsParser =
    open CustomLookupsParserTypes
    open Common
    open System.Collections.Generic

    let buildIdLabelPairLine(line: string, lineNum: int): LookupNameLine =
        let startPos = line.LastIndexOf(' ') + 1
        let typeName = line.Substring(startPos)
        let endPos = line.Length - 1
        { 
            lineNum = lineNum; 
            nameElement = Some {
                typeName = typeName;
                position = { startPos = startPos; endPos = endPos }
            }
        }

    let buildPropLine(line: string, lineNum: int, linesParsed: LinkedList<SyntaxLineType>): unit  = 
        if (line.Length > 0) then
            let words = getNextWords(line, ' ')
            if (words.Length > 0) then
                let propNameWordInfo = words.[0]
                let IdLabelPairNameElement: IdLabelPairNameElement = {
                    name =  propNameWordInfo.word;
                    position = { startPos= propNameWordInfo.startIndex; endPos= propNameWordInfo.endIndex }
                }

                if (words.Length > 1) then
                    let propTypeWordInfo = words.[1]
                    let restOfLine = propTypeWordInfo.restOfLine


                    let IdLabelPairValueElement: IdLabelPairValueElement = {
                        value =  propTypeWordInfo.word + " " + restOfLine;
                        position = { 
                                    startPos = propTypeWordInfo.startIndex;
                                    endPos = line.Length - 1 }
                    }
                             
                    linesParsed.AddLast(
                        IdLabelPairLine {
                            lineNum = lineNum;
                            nameElement = IdLabelPairNameElement;
                            valueElement = Some IdLabelPairValueElement
                        }
                    ) |> ignore
                else 
                    linesParsed.AddLast(
                        IdLabelPairLine {
                            lineNum = lineNum;
                            nameElement = IdLabelPairNameElement;
                            valueElement = None
                        }
                    ) |> ignore
    
    let typeKeyword = "lookup"
    let parseLines(lines: string seq): ParseTypesFileInfo = 

        let linesParsed: LinkedList<SyntaxLineType> = LinkedList<SyntaxLineType>()
        let customTypes: LinkedList<CustomLookupInfo> = LinkedList<CustomLookupInfo>()
        lines
        |> Seq.iteri (fun lineIdx (line: string) -> 
            if (line.Length > 0) then 
                if (line.StartsWith(typeKeyword)) then
                    linesParsed.AddLast( LookupNameLine (buildIdLabelPairLine(line, lineIdx + 2)) ) |> ignore
                else 
                    buildPropLine(line, lineIdx + 2, linesParsed)
                    
            )

            
        let mutable currType: CustomLookupInfo option = None
        linesParsed
        |> Seq.iter (fun line -> 
            match line with
            | LookupNameLine l when l.nameElement.IsSome -> 
                currType <- Some {
                            name = l.nameElement.Value.typeName ;
                            props = LinkedList<IdLabelPair>()
                        }
                currType |> Option.iter (fun s -> 
                    customTypes.AddLast(s) |> ignore 
                    )
            | IdLabelPairLine l when l.valueElement.IsSome ->
                let newProp: IdLabelPair = { id = l.nameElement.name; label = l.valueElement.Value.value }
                currType |> Option.iter (fun s -> s.props.AddLast(newProp) |> ignore )
        ) 
            
        
        { linesParsed =linesParsed; customLookups = customTypes}