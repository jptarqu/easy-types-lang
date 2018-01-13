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
    readonly nameElement: TypeNameElement
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
    readonly propTypeElement: PropTypeElement
}
export type SyntaxLineType = PropLine | TypeNameLine

export const typeKeyword = 'type'
function buildTypeNameLine(line: string, lineNum: number): TypeNameLine {
    const parts = line.split(' ')
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
function buildPropLine(line: string, lineNum: number): PropLine {
    const parts = line.split(' ')
    const startPos = line.lastIndexOf(' ') + 1
    const typeName = line.substring(startPos)
    const endPos = line.length - 1
    return {
        kind: 'PropLine',
        lineNum,
        nameElement: {
            typeName,
            position: { startPos, endPos }
        },
        propTypeElement: {

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
            linesParsed.push(buildPropLine(line, lineIdx + 1))
        }
    }
    let currType = null
    for (let lineIdx = 0; lineIdx < linesParsed.length; lineIdx++) {
        const line = linesParsed[lineIdx];
        switch (line.kind) {
            case 'TypeNameLine':
                currType = { name: line.nameElement.typeName, props: [] }
                customTypes.push(currType)
                break
            case 'PropLine':
                const newProp = { name: line.nameElement.name, propTypeName: line.propTypeElement.name }
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
    isPosATypeForProp(pos: PositionInfo, lineNumber: number) {

    }
}