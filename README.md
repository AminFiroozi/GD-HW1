GD-HW1

this is a chrome dino style endless runner me made in unity for the computer game design course. the dino runs across a desert and you gotta jump and duck past cactus and flying obstacles while collecting coins, and it gets faster the longer you survive.

![Gameplay](Screenshots/gameplay.png)

controls

jump is space, hold it a little longer and you jump a bit higher, let go early and it cuts the jump short. shift is duck and dash at same time, it speeds you up while held and if you tap shift again while in the air it makes you fall faster so you land sooner. esc pauses the game.

![Jump](Screenshots/jump.png)

![Hit](Screenshots/hit-flash.png)

features

obstacles spawn procedurally, ground cactus and flying bats and coins, and the difficulty ramps up the farther you go. jump has the variable height thing where holding longer gets you higher. dash makes you faster and the running animation also speeds up to match so it don't look weird. when you run out of lives there a death animation that plays then it waits a little bit before the restart screen actually shows up. background also got some decorative cactus and sand patches and clouds scattered around just for looks, they don't affect gameplay at all.

![Death sequence](Screenshots/death-sequence.png)

doc

the project is just the unity project folder itself, open it straight in unity hub and it should run. everything is plain c sharp scripts, no animator controller was used for the sprite animations, just arrays of sprites cycled in code which ended up being easier to manage than it sounds.
