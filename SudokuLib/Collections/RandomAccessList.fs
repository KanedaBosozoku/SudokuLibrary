module SudokuLib.RandomAccessList

open System

let indexOutOfBounds = ArgumentOutOfRangeException ("index")
let emptyList = ArgumentException ("Empty list")

type Tree<'a> =
   | Leaf of 'a
   | Node of 'a * 'a Tree * 'a Tree

type tree<'a> = Tree<'a>

module Tree =
   let rec lookup = function
      | _, Leaf x, 0 -> x
      | _, Leaf x, _ -> raise indexOutOfBounds
      | _, Node (x, _, _), 0 -> x
      | size, Node (x, left, right), index ->
         let size' = size / 2
         in
            if index <= size'
            then
               lookup (size', left, (index - 1))
            else
               lookup (size', right, index - 1 - size')

   let rec update = function
      | _, Leaf x, 0, value -> Leaf value
      | _, Leaf x, _, _ -> raise indexOutOfBounds
      | _, Node (x, left, right), 0, value -> Node (value, left, right)
      | size, Node (x, left, right), index, value ->
         let size' = size / 2
         in
            if index <= size'
            then
               Node (x, update (size', left, (index - 1), value), right)
            else
               Node (x, left, update (size', right, index - 1 - size', value))

   let rec toList = function
      | Leaf x -> [x]
      | Node (x, left, right) -> x :: toList left @ toList right

type RList<'a> =
   | Nil
   | Cons of (int * 'a tree) * RList<'a>

type rlist<'a> = RList<'a>

module RList =
   let empty = Nil

   let isEmpty = function
      | Nil -> true
      | _ -> false

   let cons x = function
      | Cons ((size1, t1), Cons ((size2, t2),rest)) as xs ->
         if size1 = size2
         then
            Cons ((1 + size1 + size2, Node (x, t1, t2)), rest)
         else
            Cons ((1, Leaf x), xs)
      | rlist -> RList.Cons ((1, Leaf x), rlist)

   let singleton x =
      cons x Nil

   let rec length = function
      | Nil -> 0
      | Cons ((size, _), rest) -> size + length rest

   let head = function
      | Nil -> raise emptyList
      | Cons ((_, Leaf x), _) -> x
      | Cons ((_, Node (x, _, _)), _) -> x

   let tail = function
      | Nil -> raise emptyList
      | Cons ((_, Leaf _), rest) -> rest
      | Cons ((size, Node (_, left, right)), rest) ->
         let size' = size / 2
         in Cons ((size', left), Cons ((size', right), rest))

   let rec nth index = function
      | Nil -> raise indexOutOfBounds
      | Cons ((size, tree), rest) ->
         if index < size
         then
            Tree.lookup (size, tree, index)
         else
            nth (index - size) rest

   let rec update index value = function
      | Nil -> raise indexOutOfBounds
      | Cons ((size, tree) as head, rest) ->
         if index < size
         then
            Cons ((size, Tree.update (size, tree, index, value)), rest)
         else
            Cons (head, update (index - size) value rest)

   let fromList list =
      List.foldBack cons list Nil

   let rec toList = function
      | Nil -> []
      | Cons ((_, tree), rest) -> Tree.toList tree @ toList rest