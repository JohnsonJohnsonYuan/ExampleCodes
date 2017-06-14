## Branch
- Create branch and switch to it:
> git checkout -b iss53

- delete branch
> git branch -d hotfix

## Log
> git log [--pretty=oneline] [–-graph]

## Show commit changes
> git show [commit_id]

## History command
> git reflog [--all]

## Reset current HEAD to the specified state
if don't specify commit para, default is HEAD, --soft will kepp current state chagnes, --hard will delete current stage files
> git reset --soft [<commit>]

> git reset --hard [<commit>]

HEAD^(HEAD~1) means the first parent of the tip of the current branch

## [Stashing and Cleaning](https://git-scm.com/book/en/v2/Git-Tools-Stashing-and-Cleaning#_git_stashing)
Usage: when you want to switch branch and you have uncomplete changes
    you don't want to commit half-done work (switch branch cannot have changes)
`Stashing are saved in stack`
Suppose you have changes on files, Now you want to switch branches, but you don’t want to commit what you’ve been working on yet; so you’ll stash the changes.
1. Step 1: stack changes
    > $ `git stash`
    Saved working directory and index state \
    "WIP on master: 049d078 added the index file"
    HEAD is now at 049d078 added the index file
    (To restore them type "git stash apply")

    Your working directory is clean:
    > $ `git status`
    #On branch master
    nothing to commit, working directory clean

1. you can easily switch branches and do work elsewhere
    - Check existing stashes:
    > `git stash list`
    stash@{0}: WIP on master: 049d078 added the index file
    stash@{1}: WIP on master: c264051 Revert "added file_size"
    stash@{2}: WIP on master: 21d80a5 added number to log

1. apply stash
    - apply most recent stash:
    > `git stash apply`

    - apply older stash
    > `git stash apply stash@{2}`


