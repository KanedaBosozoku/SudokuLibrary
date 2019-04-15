namespace SudokuLib

open System.ComponentModel
open System
open System.Globalization

type Square  =
    | Domain of Set<int>
    | Constraint of int
    | FixedConstraint of int
    | EmptyValue



type ISquare =
    abstract member IsConstraint : bool with get
    abstract member IsFixedConstraint : bool with get
    abstract member IsDomain : bool with get
    abstract member IsEmpty : bool with get

    abstract member TryGetConstraint: unit -> bool * int
    abstract member TryGetFixedConstraint: unit -> bool * int
    abstract member TryGetDomain: unit -> bool * array<int>


    abstract member Item : int with get,  set
    abstract member Items : array<int> with get, set



module SquareType =

    //open FSharpx.Collections
    open Microsoft.FSharp.Reflection


    let SquareCases = FSharpType.GetUnionCases typeof<Square>
    
    
    let EmptySquare = EmptyValue


    let SquareConverter (value : obj) =
        let v = value :?> Square
        match v with
        | Constraint(x) 
        | FixedConstraint(x) -> x :> obj
        | Domain(x) -> Set.toSeq(x) :> obj
        | _ -> 0 :> obj


    [<Literal>]
    let BlockWidthDefault = 3

    [<Literal>]
    let SymbolSet = "123456789"


    type BoardConfig =
        struct
            val BlockWidth : int
            val RowSize : int
            val GroupSize : int
            val BoardSize : int
            val Symbols : seq<char>
        

            new (blockWidth) =
                {
                    BlockWidth = blockWidth;
                    RowSize = blockWidth * blockWidth;
                    GroupSize = blockWidth * blockWidth * blockWidth;
                    BoardSize = blockWidth * blockWidth * blockWidth * blockWidth;
                
                    Symbols = seq{for c in SymbolSet.Substring(0, (blockWidth * blockWidth)) do yield c}
                }
            end

    let boardConfigDefafult = BoardConfig(BlockWidthDefault)

    let TryGetSquareConstraint(sq :Square) =
        match sq with
        | Constraint(v) -> (true, v)
        | _ -> (false, -1)


    let TryGetSquareFixedConstraint(sq :Square) =
        match sq with
        | FixedConstraint(v) -> (true, v)
        | _ -> (false, -1)

    let TryGetDoman(sq: Square) =
        match sq with
        | Domain(d) -> (true, Set.toArray d)
        | _ -> (false, Array.empty)

    let IsEmpty(sq : Square) =
        match sq with
        | EmptyValue -> (true, -1)
        | _ -> (false,-1)