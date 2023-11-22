# Video Player for Avatar VRChat
 It is video player, was designed for create video animation for avatar VRChat. Is worked according to principle enable and disable Mesh Renderer using .animation.

Installation guide and demo: https://youtu.be/G7xYz6ZQQnY

**Installation** 
1. Drag Video.Player.v1.0.unitypackage to Asset
2. On any Hierarchy object, add the "Video Player" component
3. Drag the sorted frames of your video into the Texture Array field. You can split the video itself into frames using your program or using sites like:
- https://www.aconvert.com/image/mp4-to-png/
- https://www.onlineconverter.com/video-to-jpg
4. Drag the GameObject that will be the parent for the frames. Also, do not forget to adjust the Scale, because the resolution of the video depends on it.
5. You can set your own names for the created folder with files, animations and materials.
6. Specify the duration of your video that you split into frames in step 3.1
7. Done! You now have 2 animations, ...ON.anim enables animation, OFF.anim disables accordingly.
8. When you're done, just delete the Video Player folder, because... it can cause errors when building a character.

**Preview**
![Снимок экрана 2023-11-21 140851](https://github.com/Dahus/Video-Player-for-Avatar-VRChat/assets/74374308/6603945a-ca10-4408-857f-3e12fa153f7a)
