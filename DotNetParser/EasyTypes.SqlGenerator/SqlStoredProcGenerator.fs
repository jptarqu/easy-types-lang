namespace EasyTypes.SqlGenerator

module SqlStoredProcGenerator =
    open DotNetParser.PrimitivesParserTypes
    open DotNetParser.SemanticTypes
    open SqlCommon

    
    let private isAutoCreatedDateColumn (p:TypeProperty) =
        p.name = "CreatedOn" 
    let private isAutoModifiedDateColumn (p:TypeProperty) =
        p.name = "ModifiedOn" 
    let private isAutoDateColumn (p:TypeProperty) =
        p.name = "CreatedOn" || p.name = "ModifiedOn" 
        
    let private autoGenColumn (p:TypeProperty) =
        isAutoDateColumn p || isPrimaryKey p

    let private buildParam (p:TypeProperty): string =
        let colCode = "@" + p.name + " " + convertToSqlTypeForParam(p.propType.baseType) 
        colCode
    
    let private buildCondition (p:TypeProperty): string =
        let colCode = p.name  + " = @" + p.name 
        colCode

    let nameForSpAdd  (customType: CustomType ) : string =
        "spInsert" + (customType.name)
    let buildInsertSp (customType: CustomType ) : string =
        let parameters = customType.props |> Seq.filter (autoGenColumn >> not) |> Seq.map buildParam 
        let parameterValues = 
            customType.props 
            |> Seq.filter (isPrimaryKey >> not)
            |> Seq.map (fun p -> 
                if isAutoDateColumn(p) then
                    "GetDate()"
                else
                    "@" + p.name
                )
        let header = "CREATE PROCEDURE  [" + nameForSpAdd(customType) + "]"
        "\n" + header + "\n" + (String.concat ",\n" parameters) + "\nAS\n" +
        "	
        insert into [" + customType.name + "]
	    select " + (String.concat ",\n" parameterValues) + "

	    select cast(@@IDENTITY as int) [IdGenerated]"

    let nameForSpUpdate  (customType: CustomType ) : string =
        "spUpdate" + (customType.name)
    let buildUpdateSp (customType: CustomType ) : string =
        let parameters = customType.props |> Seq.filter (isAutoDateColumn >> not) |> Seq.map buildParam 
        let conds = customType.props |> Seq.filter isPrimaryKey |> Seq.map buildCondition 
        let whereClause = if conds |> Seq.isEmpty then "" else  "where " + (String.concat "\nAND " conds) 
        let parameterValues = 
            customType.props 
            |> Seq.filter (isPrimaryKey >> not)
            |> Seq.filter (isAutoCreatedDateColumn >> not)
            |> Seq.map (fun p -> 
                if isAutoModifiedDateColumn(p) then
                    p.name + " = GetDate()"
                else
                    p.name + " = @" + p.name
                )
        let header = "CREATE PROCEDURE  [" + nameForSpUpdate(customType) + "]"
        "\n" + header + "\n" + (String.concat ",\n" parameters) + "\nAS\n" +
        "	
        update [" + customType.name + "]
	    set " + (String.concat ",\n" parameterValues) + "
         " + whereClause + "
"

    let nameForSpGet  (customType: CustomType ) : string =
        "spGet" + (customType.name) + "ById"
        

    let buildGetSp (customType: CustomType ) : string =
        let parameters = customType.props |> Seq.filter isPrimaryKey |> Seq.map buildParam 
        let conds = customType.props |> Seq.filter isPrimaryKey |> Seq.map buildCondition 
        let selectedCols = 
            customType.props 
            |> Seq.map (fun p ->  p.name
                )
        let header = "CREATE PROCEDURE  [" + nameForSpGet(customType) + "]"
        let whereClause = if conds |> Seq.isEmpty then "" else  "where " + (String.concat "\nAND " conds) 
        "\n" + header + "\n" + (String.concat ",\n" parameters) + "\nAS\n" +
        "	
	    select TOP 1 " + (String.concat ",\n" selectedCols) + " from [" + customType.name + "] " + whereClause
        
