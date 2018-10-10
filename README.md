# Adsophic.Framework.WordProcessing
## Introduction
In this repository we tackle some common word related algorithms. One of the common problems that we may face while processing text is - how do we efficiently confirm the presence of a word in a language. In other words, given a string, we want to check if that string represents an existing word in a language. Another problem is autocorrection. Given a word how do we find words closest to that word to provide autocorrect suggestions. 

## Solution Structure
Solution consists of two projects
- Adsophic.Framework.WordProcessing
- Adsophic.Framework.WordProcessing.Test

As you would expect, *Adsophic.Framework.WordProcessing* contains implementation. Class *Adsophic.Framework.WordProcessing.WordSearch* exposes implementation to the outside world. It creates and maintains references to algorithms. Algorithms are themselves implemented in classes *Adsophic.Framework.WordProcessing.Trei* and *Adsophic.Framework.WorkProcessing.AutoCorrect*.

*Adsophic.Framework.WordProcessing.Test* contains relevant tests.

## Algorithms
### Trei - Find matching words
One of the efficient ways to check if a word exists in a dictionary is via use of a *Trei* data structure. In this algorithm, we create *trei* structure that represents all words in the dictionary. After initialization, we can traverse it for any given string input to confirm if it matches a word in the dictionary. One aspect of the algorithm warrants further explanation. When creating the Trei we mark the node representing the last letter in a word with the flag *IsWord = true*. We need this to prevent Trei from returning *true* if word to check prefixes another word. For example, if we were to search for *cau* our traversal would be successful to the last letter since *cau* prefixes *caution*. But *cau* is not a word. Using *IsWord* prevents it from happening, since only the node representing the last letter when inserting *caution* will have its *IsWord* set to true, the node representing the letter *u* wouldn't. *Trei.FindWord* therefore returns value of *IsWord* we successfully traverse to the last letter. 

*FindWordsFromCharacters* is something scrabble players might find useful. I'd read a story of how the Facebook founder decided to write a program to help him figure out words in scrabble. Thought I'd give it a go. Algorithm checks for possible matching words for all possible permutations of indvidual letters. 

### AutoCorrect - Find closest words
Here we use the Levishtan distance algorithm. This algorithm returs the minimum number of edits it would take to get from one word to another. We are also restricting the search to words that begin with the same letter as the word we are searching for. We are making an assumption that users know the first letter of the word they are trying to spell. But be aware that means that *ecause* will not match *because* even though it is only one edit distance away. We also restrict search to words with wordlength +- 1.  


