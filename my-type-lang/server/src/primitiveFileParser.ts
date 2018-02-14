import { PositionInfo, getIndexAfterToken } from './common';

export interface PrimitiveNameElement {
    readonly name: string,
    readonly position: PositionInfo
}
export interface BasePrimitiveTypeElement {
    readonly name: string,
    readonly position: PositionInfo
}
export interface BasePrimtiveArgElement {
    readonly value: string,
    readonly position: PositionInfo
}

export interface PrimitiveLine {
    readonly kind: 'PrimitiveLine',
    readonly lineNum: number,
    readonly nameElement: PrimitiveNameElement,
    readonly primitveBaseTypeElement?: BasePrimitiveTypeElement
    readonly baseArguments: BasePrimtiveArgElement[]
}

export enum BaseTypes {
    text = 'text',
    integer = 'integer',
    decimal = 'decimal',
    date = 'date',
    dateAndTime = 'dateAndTime'
}
type PrimitiveSyntaxLineType = PrimitiveLine

interface WordInfo {
    readonly word: string
    readonly restOfLine: string
    readonly startIndex: number
    readonly endIndex: number
}
function getNextWord(line: string, wordBoundary: string): WordInfo {
    let startIndex = 0
    if (line[0] === wordBoundary) {
        startIndex = getIndexAfterToken(line, wordBoundary)
    }
    const firstWordEndIdx = line.indexOf(wordBoundary, startIndex)
    if (firstWordEndIdx > 1) {
        const word = line.substring(startIndex, firstWordEndIdx)
        const restOfLine = line.substring(firstWordEndIdx)
        return { word, restOfLine, startIndex, endIndex: firstWordEndIdx }
    } else {
        return { word: '', restOfLine: line, startIndex, endIndex: 0 }
    }

}
function getNextWords(line: string, wordBoundary: string): WordInfo[] {
    const words: WordInfo[] = []
    let currWord: WordInfo = getNextWord(line, wordBoundary)
    while (currWord.word !== '') {
        words.push(currWord)
        currWord = getNextWord(line, wordBoundary)
    }
    return words
}
function buildPropLine(line: string, lineNum: number, linesParsed: PrimitiveSyntaxLineType[]): void {
    if (line.length > 0) {
        const words = getNextWords(line, ' ')
        if (words.length > 0) {
            const primitiveWordInfo = words[0]
            const nameElement: PrimitiveNameElement = {
                name: primitiveWordInfo.word,
                position: { startPos: 0, endPos: primitiveWordInfo.endIndex }
            }
            let baseTypeElement: BasePrimitiveTypeElement
            let baseArguments: BasePrimtiveArgElement[] = []
            if (words.length > 1) {
                const baseTypeWordInfo = words[1]
                baseTypeElement = {
                    name: baseTypeWordInfo.word,
                    position: {
                        startPos: baseTypeWordInfo.startIndex,
                        endPos: baseTypeWordInfo.endIndex
                    }
                }
                if (words.length > 2) {
                    const argsWords = words.slice(2)
                    for (let argIdx = 0; argIdx < argsWords.length; argIdx++) {
                        const argWord = argsWords[argIdx];
                        baseArguments.push(
                            {
                                value: argWord.word,
                                position: {
                                    startPos: argWord.startIndex,
                                    endPos: argWord.endIndex
                                }
                            }
                        )
                    }
                }
            }
            linesParsed.push(
                {
                    kind: 'PrimitiveLine',
                    lineNum: lineNum,
                    nameElement,
                    primitveBaseTypeElement: baseTypeElement,
                    baseArguments
                }
            )
        }

    }
}
export interface CustomPrimitive {
    readonly name: string
    readonly baseType: string
    readonly baseArgs: string[]
}
export interface ParsePrimtivesFileInfo {
    readonly linesParsed: PrimitiveSyntaxLineType[]
    readonly customPrimitives: CustomPrimitive[]
}
export function parsePrimtivesLines(lines: string[]): ParsePrimtivesFileInfo {

    const linesParsed: PrimitiveSyntaxLineType[] = []
    const customPrimitives: CustomPrimitive[] = []
    for (let lineIdx = 1; lineIdx < lines.length; lineIdx++) {
        const line = lines[lineIdx];
        if (line.length > 0 && !line.startsWith(' ')) {
            buildPropLine(line, lineIdx + 1, linesParsed)
        }
    }
    let currType: CustomPrimitive = null
    for (let lineIdx = 0; lineIdx < linesParsed.length; lineIdx++) {
        const line = linesParsed[lineIdx];
        switch (line.kind) {
            case 'PrimitiveLine':
                currType = {
                    name: line.nameElement.name || '',
                    baseType: line.primitveBaseTypeElement ? line.primitveBaseTypeElement.name : '',
                    baseArgs: line.baseArguments.map((a) => a.value)
                }
                customPrimitives.push(currType)
                break
            default:
                break
        }
    }
    return {
        linesParsed, customPrimitives
    }
}