using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserDto
{

    public string id;

    public string firstname;

    public string lastname;

    [SerializeField]
    private string birthday;
    private DateTime? _birthday;

    public DateTime? Birthday()
    {
        if (_birthday == null)
        {
            _birthday = Convert.ToDateTime(birthday);
        }
        return _birthday;
    }

    public string professionalOccupation;

    public string twittername;

    public FaceRectangle faceRectangle;

    public TweetDto[] tweets;


}


[Serializable]
public class FaceRectangle
{

    public int width;
    public int Height;
    public int left;
    public int top;
}



[Serializable]
public class UserDtoCollection
{
    public UserDto[] data;
}

[Serializable]
public class TweetDto
{
    public string tweet;
}




