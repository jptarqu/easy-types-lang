namespace DotNetParser

module Common = 
    open System.Collections.Generic

    type PositionInfo = {
        startPos: int
        endPos: int
    }
    
    type WordInfo =  {
        word: string
        restOfLine: string
        startIndex: int
        endIndex: int
    }

    let getIndexAfterToken(str: string, token: char) = 
        let mutable idx = 0
        seq {
            for char in str do
                if (char <> token) then
                    yield idx
                idx <- idx + 1
            yield idx
        }
        |> Seq.head
    
    let getNextWord(line: string, wordBoundary: char): WordInfo =
        let mutable startIndex = 0
        if (line.[0] = wordBoundary) then
            startIndex <- getIndexAfterToken(line, wordBoundary)
        let firstWordEndIdx = line.IndexOf(wordBoundary, startIndex)
        if (firstWordEndIdx > 1) then
            let word = line.Substring(startIndex, firstWordEndIdx)
            let restOfLine = line.Substring(firstWordEndIdx)
            { word = word; restOfLine = restOfLine; startIndex = startIndex; endIndex = firstWordEndIdx }
        else
            { word =  ""; restOfLine = line; startIndex = startIndex; endIndex=  0 }
        
    let getNextWords(line: string, wordBoundary: char): WordInfo[] =
        let words = LinkedList<WordInfo>()
        let currWord: WordInfo = getNextWord(line, wordBoundary)
        while (currWord.word <> "") do
            words.AddLast(currWord) |> ignore
        words |> Seq.toArray
    