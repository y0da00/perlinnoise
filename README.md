# perlinnoise
This was created using .NET and Raylib-cs. It feuters a perlin noise and fractal noise algorithms. It should run at mostly decent PFS at most computers if not it will run at the fabulous 1 FPS. I tried optimizing what was possible to make it faster but there is a lot more to be done.
There is an animation mode and a normal mode. The animation just rotates the gradient vectors with a step every frame so the higher fps the better it will look. 

If you want you can turn the animation off by holding F and turn it back on by holding T. There is also fractal noise which is just several layers of perlin noise but at different frequences and amplitudes. 

If you want to see the fractal noise just change the 'noise' variable to 'fracNoise' at line 151.
Also if you close the window and press any key you will get the noise visualized in the console.
