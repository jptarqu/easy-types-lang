namespace DotNetParser

module CustomTypesParser =
    open CustomTypesParserTypes
    open Common
    open System.Collections.Generic

    let buildTypeNameLine(line: string, lineNum: int): TypeNameLine =
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
                let propNameElement: PropNameElement = {
                    name =  propNameWordInfo.word;
                    position = { startPos= propNameWordInfo.startIndex; endPos= propNameWordInfo.endIndex }
                }

                if (words.Length > 1) then
                    let propTypeWordInfo = words.[1]
                    let propTypeElement: PropTypeElement = {
                        name =  propTypeWordInfo.word;
                        position = { 
                                    startPos = propTypeWordInfo.startIndex;
                                    endPos = propTypeWordInfo.endIndex }
                    }
                             
                    linesParsed.AddLast(
                        PropLine {
                            lineNum = lineNum;
                            nameElement = propNameElement;
                            propTypeElement = Some propTypeElement
                        }
                    ) |> ignore
                else 
                    linesParsed.AddLast(
                        PropLine {
                            lineNum = lineNum;
                            nameElement = propNameElement;
                            propTypeElement = None
                        }
                    ) |> ignore
    
    let typeKeyword = "type"
    let parseTypesLines(lines: string seq): ParseTypesFileInfo = 

        let linesParsed: LinkedList<SyntaxLineType> = LinkedList<SyntaxLineType>()
        let customTypes: LinkedList<CustomTypeInfo> = LinkedList<CustomTypeInfo>()
        lines
        |> Seq.iteri (fun lineIdx (line: string) -> 
            if (line.Length > 0) then 
                if (line.StartsWith(typeKeyword)) then
                    linesParsed.AddLast( TypeNameLine (buildTypeNameLine(line, lineIdx + 2)) ) |> ignore
                else 
                    buildPropLine(line, lineIdx + 2, linesParsed)
                    
            )

            
        let mutable currType: CustomTypeInfo option = None
        linesParsed
        |> Seq.iter (fun line -> 
            match line with
            | TypeNameLine l when l.nameElement.IsSome -> 
                currType <- Some {
                            name = l.nameElement.Value.typeName ;
                            props = LinkedList<Prop>()
                        }
                currType |> Option.iter (fun s -> 
                    customTypes.AddLast(s) |> ignore 
                    )
            | PropLine l when l.propTypeElement.IsSome ->
                let newProp: Prop = { name = l.nameElement.name; propTypeName = l.propTypeElement.Value.name }
                currType |> Option.iter (fun s -> s.props.AddLast(newProp) |> ignore )
        ) 
            
        
        { linesParsed =linesParsed; customTypes = customTypes}