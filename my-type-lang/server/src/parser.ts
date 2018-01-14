export interface Prop {
    readonly name: string,
    readonly propTypeName: string,
}
export interface CustomType {
    readonly name: string,
    readonly props: Prop[]
}

export interface PositionInfo {
    readonly startPos: number,
    readonly endPos: number,
}

export interface TypeNameElement {
    typeName: string,
    position: PositionInfo
}

export interface TypeNameLine {
    readonly kind: 'TypeNameLine',
    readonly lineNum: number,
    readonly nameElement?: TypeNameElement
}

export interface PropNameElement {
    readonly name: string,
    readonly position: PositionInfo
}
export interface PropTypeElement {
    readonly name: string,
    readonly position: PositionInfo
}
export interface PropLine {
    readonly kind: 'PropLine',
    readonly lineNum: number,
    readonly nameElement: PropNameElement,
    readonly propTypeElement?: PropTypeElement
}
export type SyntaxLineType = PropLine | TypeNameLine

export const typeKeyword = 'type'
function buildTypeNameLine(line: string, lineNum: number): TypeNameLine {
    const startPos = line.lastIndexOf(' ') + 1
    const typeName = line.substring(startPos)
    const endPos = line.length - 1
    return {
        kind: 'TypeNameLine', lineNum, nameElement: {
            typeName,
            position: { startPos, endPos }
        }
    }
}
function getIndexAfterToken(str: string, token: string) {
    let idx = 0
    for (var char of str) {
        if (char !== token) {
            return idx
        }
        idx++
    }
    return idx
}
function buildPropLine(line: string, lineNum: number, linesParsed: SyntaxLineType[]): void {
    if (line.length > 0) {
        // vscode converts tabs into spaces
        const firstWordIdx = getIndexAfterToken(line, ' ')

        const endPos = line.length - 1
        if (firstWordIdx > 0) {
            const propLine = line.substring(firstWordIdx).trim()
            const nameEndIdx = propLine.indexOf(' ')
            const name = propLine.substring(0, nameEndIdx)
            const nameElement: PropNameElement = {
                name,
                position: { startPos: firstWordIdx, endPos: nameEndIdx }
            }
            const restOfLine = propLine.substring(nameEndIdx)

            let propTypeElement: PropTypeElement
            if (restOfLine.trim() !== '') {
                const nextWordIdx = getIndexAfterToken(restOfLine, ' ')
                const typeName = propLine.substring(nextWordIdx).trim()
                propTypeElement = {
                    name: typeName,
                    position: { startPos: nextWordIdx, endPos }
                }
            }
            linesParsed.push({
                kind: 'PropLine',
                lineNum,
                nameElement,
                propTypeElement
            })
        }
    }
}
export interface ParseFileInfo {
    readonly linesParsed: SyntaxLineType[]
    readonly customTypes: CustomType[]
}
function parseLines(lines: string[]): ParseFileInfo {
    const linesParsed: SyntaxLineType[] = []
    const customTypes: CustomType[] = []
    for (let lineIdx = 0; lineIdx < lines.length; lineIdx++) {
        const line = lines[lineIdx];
        if (line.startsWith(typeKeyword)) {
            linesParsed.push(buildTypeNameLine(line, lineIdx + 1))
        } else {
            buildPropLine(line, lineIdx + 1, linesParsed)
        }
    }
    let currType = null
    for (let lineIdx = 0; lineIdx < linesParsed.length; lineIdx++) {
        const line = linesParsed[lineIdx];
        switch (line.kind) {
            case 'TypeNameLine':
                currType = { name: line.nameElement.typeName || '', props: [] }
                customTypes.push(currType)
                break
            case 'PropLine':
                const newProp = {
                    name: line.nameElement.name || '', propTypeName:
                        line.propTypeElement.name || ''
                }
                currType.props.push(newProp)
                break
            default:
                break
        }
    }
    return {
        linesParsed, customTypes
    }
}

export class TypeParser {
    private currParseFileInfo: ParseFileInfo
    parse(lines: string[]) {
        this.currParseFileInfo = parseLines(lines)
    }
    isPosATypeForProp(charNum: number, lineNumber: number) {
        const syntaxLine = this.currParseFileInfo.linesParsed[lineNumber]
        if (!syntaxLine) {
            return false
        }
        return (syntaxLine.nameElement.position.endPos + 1) < charNum
    }
}