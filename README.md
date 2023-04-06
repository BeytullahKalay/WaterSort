### Water Sort Puzzle Game
This game is a puzzle game where we try to collect the same colors in the same bottles.
## Game Goal
collect the same colors in the same bottles.
## Game Link
You can easily download the game from [PlayStore](https://play.google.com/store/apps/details?id=com.watersortpuzzle.colorsortpuzzle.watercolorsortpuzzle&hl=en&gl=us)
## Design Details

After tutorial(after level 4) levels generating. I can't design levels so I decided to write a level generator.
The generator have conditions like how many bottle should have in level, which color will be create, can create same color in a bottle and so on. 
After level generator I needed to make sure the generated levels were solvable. I coded a solver. If level cannot solve after 20.000 move,
generator creating new level.
