namespace SudokuLib
//open FSharpx.Collections
open SudokuLib.RandomAccessList

//type Puzzle = RandomAccessList<Square>
type Puzzle = RList<Square>

type Domains =
 {
    Row : Set<int> [];
    Col : Set<int> [];
    Blk : Set<int> [];
 }

module BoardType =
    open SquareType
    

    open System.Collections.Specialized
    open System.ComponentModel
    open System.Collections

    open System



    let CreateSudokuBoard(data : seq<Square>) =
        RList.fromList (Seq.toList data)


    let PadHints (nums : int list) : int list =
        let rec padHintsAux lst idx acc =
            match lst with 
            | a::_ when a < idx ->
                padHintsAux lst (idx + 1) (0::acc)

            | a::rest when a = idx ->
                padHintsAux rest (idx + 1) (a::acc)

            | nil when idx < 10 ->
                padHintsAux nil (idx + 1) (0::acc)
            | nil ->
                (List.rev acc)
        padHintsAux nums 1 []


    let (^) l r = sprintf "%s%s" l r

    let PadToString (nums : int list) : string =
        let rec padToStringAux lst acc =
            match lst  with
            | a::rest when a > 0 -> padToStringAux rest (acc ^ sprintf "%A" a)
            | a::rest when a = 0 -> padToStringAux rest (acc ^ " ")
            | _ -> acc
        padToStringAux nums ""


    let PadHintToString (nums) =
        nums
        |> PadHints
        |> PadToString



    //let getDomain (index : seq<int>) (board : RandomAccessList<Square>) =
    //    Seq.map ((fun idx -> board.[idx]) >> (fun x ->
    //        match x with
    //        | Constraint(v)
    //        | FixedConstraint(v) -> v
    //        | _ -> 0)) index


    let getDomain (index : seq<int>) (board : RList<Square>) =
        index |>
        Seq.choose ((fun idx -> (RList.nth idx board)) >> (fun x ->
            match x with
            | Constraint(v)
            | FixedConstraint(v) -> Some(v)
            | _ -> None)) 


    let getDomains (indexFun : int->seq<int>) (board) =
        (Seq.map ((fun i ->  (getDomain (indexFun i) board)) >> (Set.ofSeq)) (seq{0..8}))
        |> Array.ofSeq

    let getRowDomains (board) =
        getDomains IndexMapping.rowIndexSeq board

    let getColumnDomains (board) =
        getDomains IndexMapping.columnIndexSeq board

    let getBlockDomains (board) =
        getDomains IndexMapping.blockIndexSeq board



    let printBoard (board : RList<Square>) =
        seq{0..80}
        |> Seq.map (fun idx -> (RList.nth idx board))
        |> Seq.chunkBySize 9
        |> Seq.iter (fun x ->
            x |> Array.iter (fun y ->
                match y with
                | Constraint(a)
                | FixedConstraint(a) -> (printf " %d " a)
                | _  -> (printf " . "))
            printfn "")


    ///Find cells that have only one domain value for indexes
    
    let FindSingle (indexes : seq<int>) (board : RList<Square>) =
        indexes
        |> Seq.choose(fun idx ->
            match (RList.nth idx board) with
            | Domain(d) when d.Count = 1 -> Some(idx)
            | _ -> None)

    ///Find cells that have only one domain value useing thet indexer function

    let FindSingles (indexer : int -> seq<int>) (board : RList<Square>) =
        seq{0..8}
        |> Seq.collect(fun i ->
            FindSingle (indexer i) board)
    
    let get (indexes : seq<int>) (board : RList<Square>) =
        indexes
        |> Seq.choose (fun i->
            match (RList.nth i board) with
            | Domain(s) -> Some(i, Set.toList s)
            | _ -> None)

    let folder (acc : array<int*int*int>) (v : int * List<int>) =
        let bIdx, nums = v
        nums 
        |> List.iter (fun i ->
            let _, cnt, _ = acc.[i]
            acc.[i] <- (i, cnt+1, bIdx))
        acc

        
    let FindHidden (indexes : seq<int>) (board : RList<Square>) =
        get indexes board
        |> Seq.fold folder (Array.create 9 (0,0,0))
        |> Array.choose(fun (sym,cnt,idx) -> 
            match cnt with
            | 1 -> Some(sym, idx)
            | _ -> None)


    let InitializeAllDomains (board) =
         { Row = (getRowDomains board);
           Col = (getColumnDomains  board);
           Blk = (getBlockDomains  board)}



    /// <summary>computes the Domain value of a cell using the row, column, and block set
    /// initialized with the constants values of the board
    /// <Summary
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="blk"></param>
    /// <param name="idx"></param>
    let checkCover (domains:Domains) idx =
        let sym = [1..9] |> Set.ofList
        Set.union domains.Blk.[IndexMapping.indexToBlockNumber idx] 
                (Set.union domains.Row.[IndexMapping.indexToRowNumber idx] 
                domains.Col.[IndexMapping.indexToColumnNumber idx]) |> Set.difference sym



    let ComputeDomain (board)  =
        let cover = (InitializeAllDomains board) |> checkCover
        RList.toList board
        |> List.mapi (fun idx l ->
            match l with
            | EmptyValue -> Domain(cover idx)
            | _ -> l)



    let RecomputeDomains (puzzle : Puzzle) position t =
        let rec change (p : Puzzle) w lst =
            match lst with
            | a::b -> 
                let u  = match (RList.nth a p) with 
                         | Domain(o) -> o.Remove(w) 
                         | _ -> raise (Exception())

                //change (p.Update(a, Domain(u))) w b 
                change (RList.update a (Domain(u)) p) w b // p.Update(a, Domain(u))) w b 
            | [] -> p

        let findReplace h  = 
            h
            |> Seq.filter(fun x ->
                match (RList.nth x puzzle) with
                | Domain(k) -> k.Contains(t)
                | _-> false)
            |> Seq.distinct
            |> Seq.toList
            
        change puzzle t (findReplace (IndexMapping.squaresAffected position))

