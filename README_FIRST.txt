Read all the comments in all the files before attempting.

In a nutshell,
Make sure to 
   i) put your clientid and developerkey in BrowserUpload.vb, yt.vb and AuthSub.vb files
   ii) put your username and password on youtube if using yt.vb
   iii) change nexturl in TestAuthSub.aspx and Default2.aspx (HTML SOURCE VIEW, NOT CODE BEHIND)
   iv) Change NextUrl in AuthSub.vb file (class level variable) to a page that you want users to redirected to after they authenticate on Google. Do not 
       confuse this with nexturl in point iii above. The nexturl in point iii is the url that users are redirected to after the video is uploaded.
    v) Default.aspx uses yt.vb. yt.vb is only used for direct uploading and not suitable to be used in a scenario where you want your users to upload. Read comments in yt.vb
       


You can also place clientid and developerkey in web.config. 


Make sure to read complete comments on each file.
If any questions, rafayali@gmail.com

