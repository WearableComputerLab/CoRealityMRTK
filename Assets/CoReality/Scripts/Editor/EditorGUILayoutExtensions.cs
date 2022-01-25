using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

//Here is a static class to hold my gui stuff.

    public static class EditorGUILayoutExtensions
    {

        //Alternate Method
        public static bool LinkLabel(string labelText)
        {
            return LinkLabel(labelText, Color.black, new Vector2(), 0);
        }

        //Alternate Method
        public static bool LinkLabel(string labelText, Color labelColor)
        {
            return LinkLabel(labelText, labelColor, new Vector2(), 0);
        }

        //Alternate Method
        public static bool LinkLabel(string labelText, Color labelColor, Vector2 contentOffset)
        {
            return LinkLabel(labelText, labelColor, contentOffset, 0);
        }

        //The Main Method
        public static bool LinkLabel(string labelText, Color labelColor, Vector2 contentOffset, int fontSize)
        {
            //Let's use Unity's label style for this
            GUIStyle stl = EditorStyles.label;
            //Next let's record the settings for Unity's label style because we will have to make sure these settings get returned back to
            //normal after we are done changing them and drawing our LinkLabel.
            Color col = stl.normal.textColor;
            Vector2 os = stl.contentOffset;
            int size = stl.fontSize;
            //Now we can modify the label's settings via the editor style : EditorStyles.label (stl).
            stl.normal.textColor = labelColor;
            stl.contentOffset = contentOffset;
            stl.fontSize = fontSize;
            //We are now ready to draw our Linklabel. I will actually use a GUILayout.Button to do this and our "stl" style will
            //make the button appear as a label.

            //Note : You may include a web address parameter in this method and open a URL at this point if the button is clicked,
            //however, I am going to just return bool based on weather or not the link was clicked. This gives me more control over
            //what actually happens when a link label is used. I also will instead include a "URL version" of this method below.

            //Since the button already returns bool, I will just return that result straight across like this.

            try
            {
                return GUILayout.Button(labelText, stl);
            }
            finally
            {
                //Remember to set the editor style (stl) back to normal here. A try / finally clause will work perfectly for this!!!

                stl.normal.textColor = col;
                stl.contentOffset = os;
                stl.fontSize = size;
            }
        }

        //This is a modified version of link label that opens a URL automatically. Note : this can also return bool if you want.
        public static void LinkLabel(string labelText, Color labelColor, Vector2 contentOffset, int fontSize, string webAddress)
        {
            if (LinkLabel(labelText, labelColor, contentOffset, fontSize))
            {
                try
                {
                    Application.OpenURL(@webAddress);
                    //if returning bool, return true here.
                }
                catch
                {
                    //In most cases, the catch clause would not happen but in the interest of being thorough I will log an
                    //error and have Unity "beep" if an exception gets thrown for any reason.
                    Debug.LogError("Could not open URL. Please check your network connection and ensure the web address is correct.");
                    EditorApplication.Beep();
                }
            }
            //if returning bool, return false here.
        }
    }

