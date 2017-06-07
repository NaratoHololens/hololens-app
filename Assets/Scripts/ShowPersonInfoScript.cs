using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPersonInfoScript : MonoBehaviour
{

    public Text user1;
    public Text user2;
    public Text user3;
    public Text user4;
    public Text user5;
    public Text userName;
    public Text userBirthday;
    public Text userOccupation;
    public Text tweet;

    private List<Text> textUsers;
    private GameObject panelCurrentUser;
    private Queue<UserDto> Persons;
    private const string scanningText = "Scanning...";
    private const int panelUpdateInterval = 6;
    private double elapsedTime = 0;
    private int listIndex = 0;

    // Use this for initialization
    void Start()
    {
        panelCurrentUser = GameObject.Find("PanelCurrent");
        panelCurrentUser.SetActive(false);
        Persons = new Queue<UserDto>();
        user1.text = scanningText;
        elapsedTime = 0;
        textUsers = new List<Text>();
        textUsers.AddRange(new Text[]{ user1, user2, user3, user4, user5});
    }

    // Update is called once per frame
    void Update()
    {
        if (elapsedTime >= 1)
        {
            elapsedTime = 0;
            ChangePerson();
        }
        else
        {
            elapsedTime += Time.deltaTime;
        }
    }

    private IEnumerator UpdatePanel()
    {
        while (true)
        {
            ChangePerson();
            yield return new WaitForSeconds(panelUpdateInterval);
        }
    }

    public void UpdatePersonList(UserDto[] users)
    {
        if (users.Count() != 0)
        {
            textUsers.ForEach(t => t.text = "");
            user1.text = "";
            listIndex = users.Count()-1;
            Persons.Clear();
            foreach (var userDto in users)
            {
                Persons.Enqueue(userDto);
            }

            if (users.Count() >= 1)
            {
                setUserinfoInText(user1, users[0]);
                if (users.Count() >= 2)
                {
                    setUserinfoInText(user2, users[1]);
                    if (users.Count() >= 3)
                    {
                        setUserinfoInText(user3, users[2]);
                        if (users.Count() >= 4)
                        {
                            setUserinfoInText(user4, users[3]);
                            if (users.Count() >= 5)
                                setUserinfoInText(user5, users[4]);
                        }
                    }
                }
            }
        }
        /*else if (Persons.Count() == 0)
        {
            userPane.text = scanningText;
        }*/
        //changePerson();

    }

    private void setUserinfoInText(Text userTxt, UserDto user)
    {
        if(user != null)
        {
            userTxt.text = $"{user.firstname} {user.lastname}\n";
        }
    }

    private void ChangePerson()
    {
        if (Persons.Count != 0)
        {
            UserDto userDto = Persons.Dequeue();
            Persons.Enqueue(userDto);//put user at the end of the list so the list doesn't get empty
            textUsers.ElementAt(listIndex).color = Color.white;
            listIndex += 1;
            if (listIndex > Persons.Count-1 || listIndex > 4) {
                listIndex = 0;
            }
            textUsers.ElementAt(listIndex).color = Color.yellow;
            panelCurrentUser.SetActive(true);

            userName.text = userDto.firstname + " " + userDto.lastname;
            string birthday = userDto.Birthday().ToString();
            int index = birthday.LastIndexOf("/") + 5;
            userBirthday.text = "Birthday: " + birthday.Substring(0, index);
            userOccupation.text = "Occupation: " + userDto.professionalOccupation;
            tweet.text = userDto.tweets.First().tweet;
            //tweets = userDto.tweets;
            //test = jsonResults;

            //string recognized = userDto.firstname + " " + userDto.lastname + " was here!\n";
        }/*
        else
        {
            panelCurrentUser.SetActive(false);
        }*/
    }
}
