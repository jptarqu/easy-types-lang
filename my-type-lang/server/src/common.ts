
export interface PositionInfo {
    readonly startPos: number,
    readonly endPos: number,
}

export function getIndexAfterToken(str: string, token: string) {
    let idx = 0
    for (var char of str) {
        if (char !== token) {
            return idx
        }
        idx++
    }
    return idx
}