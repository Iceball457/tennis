using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuStatRanking : MonoBehaviour {
    private const float POWER_IMPORTANCE = 15f;
    private const float CONTROL_IMPORTANCE = 5.5f;
    private const float SPIN_IMPORTANCE = 3f;
    private const float UP_FORCE_IMPORTANCE = 5.75f;
    private const float SPEED_IMPORTANCE = 15f;
    private const float DASH_IMPORTANCE = 7f;
    private const float DEXTERITY_IMPORTANCE = 25f;
    private const float AGILITY_IMPORTANCE = 12f;
    private const float STOP_IMPORTANCE = 30f;

    public GameObject selection;
    public GameObject readyIndicator;
    GameObject menu;
    public int menuHeight;
    public bool ready; 

    public string horizontal;
    public string vertical;
    public string selectAxis;
    public string backAxis;
    float horizontalBuffer;
    float verticalBuffer;
    bool selectBuffer;
    bool backBuffer;
    public int startMenuHeight;
    bool up;
    bool down;
    bool right;
    bool left;
    bool select;
    bool back;

    private void Start () {
        menu = GameObject.Find ("Selections");
        if (startMenuHeight < 0) {
            startMenuHeight = 0;
        }
        if (startMenuHeight > menu.transform.childCount - 1) {
            startMenuHeight = menu.transform.childCount - 1;
        }
        menuHeight = startMenuHeight;
    }

    private void Update () {
        GetInputs ();
        if (!ready) {
            if (left) {
                if (menuHeight > 0) {
                    menuHeight -= 1;
                }
            } else if (right) {
                if (menuHeight < menu.transform.childCount - 1) {
                    menuHeight += 1;
                }
            } else if (up) {
                if (menuHeight <= menu.transform.childCount - 5) {
                    menuHeight += 4;
                }
            } else if (down) {
                if (menuHeight > 4) {
                    menuHeight -= 4;
                }
            }
            if (select) {
                ready = true;
            }
            readyIndicator.SetActive (false);
        } else {
            if (back) {
                ready = false;
            }
            readyIndicator.SetActive (true);
        }

        GetComponent<Image> ().sprite = menu.transform.GetChild (menuHeight).GetComponent<Image> ().sprite;
        HighlightUIElement (menu.transform.GetChild (menuHeight).gameObject);
        Rank (menu.transform.GetChild (menuHeight).GetComponent<CharacterDataReference> ().data);
    }

    private void GetInputs () {
        if (Input.GetAxis (horizontal) > 0 && horizontalBuffer == 0) {
            right = true;
        } else {
            right = false;
        }
        if (Input.GetAxis (horizontal) < 0 && horizontalBuffer == 0) {
            left = true;
        } else {
            left = false;
        }
        if (Input.GetAxis (vertical) > 0 && verticalBuffer == 0) {
            up = true;
        } else {
            up = false;
        }
        if (Input.GetAxis (vertical) < 0 && verticalBuffer == 0) {
            down = true;
        } else {
            down = false;
        }
        if (Input.GetAxis (selectAxis) > 0 && !selectBuffer) {
            select = true;
        } else {
            select = false;
        }
        if (Input.GetAxis (backAxis) > 0 && !backBuffer) {
            back = true;
        } else {
            back = false;
        }
        if (Input.GetAxis (backAxis) > 0) {
            backBuffer = true;
        } else {
            backBuffer = false;
        }
        if (Input.GetAxis (selectAxis) > 0) {
            selectBuffer = true;
        } else {
            selectBuffer = false;
        }
        horizontalBuffer = Input.GetAxis (horizontal);
        verticalBuffer = Input.GetAxis (vertical);
    }
    public void Rank (PlayerController character) {
        gameObject.transform.GetChild (0).GetComponent<Image> ().fillAmount = RankPower (character.power, character.control);
        gameObject.transform.GetChild (1).GetComponent<Image> ().fillAmount = RankCurve (character.spin);
        gameObject.transform.GetChild (2).GetComponent<Image> ().fillAmount = RankLob (character.upForce, character.control);
        gameObject.transform.GetChild (3).GetComponent<Image> ().fillAmount = RankSpeed (character.speed, character.dashThreshhold);
        gameObject.transform.GetChild (4).GetComponent<Image> ().fillAmount = RankAgility (character.dexterity, character.agility, character.stopForce);
    }
    float RankPower (float _power, float _control) {
        float output = 0;
        output = _power / POWER_IMPORTANCE * 0.7f;
        output += _control / CONTROL_IMPORTANCE * 0.3f;
        output = Mathf.Clamp01 (output);
        output = Mathf.Pow (output, 2f);
        return output;
    }
    float RankCurve (float _spin) {
        float output = 0;
        output = _spin / SPIN_IMPORTANCE;
        output = Mathf.Clamp01 (output);
        output = Mathf.Pow (output, 2f);
        return output;
    }
    float RankLob (float _upForce, float _control) {
        float output = 0;
        output = _upForce / UP_FORCE_IMPORTANCE * 0.9f;
        output += _control / CONTROL_IMPORTANCE * 0.1f;
        output = Mathf.Clamp01 (output);
        output = Mathf.Pow (output, 2f);
        return output;
    }
    float RankSpeed (float _speed, float _dashThreshhold) {
        float output = 0;
        output = _speed / SPEED_IMPORTANCE * 0.8f;
        output += _dashThreshhold / DASH_IMPORTANCE * 0.2f;
        output = Mathf.Clamp01 (output);
        output = Mathf.Pow (output, 2f);
        return output;
    }
    static float RankAgility (float _dexterity, float _agility, float _stopForce) {
        float output = 0;
        output = _dexterity / DEXTERITY_IMPORTANCE * 0.4f;
        output += _agility / AGILITY_IMPORTANCE * 0.4f;
        output += _stopForce / STOP_IMPORTANCE * 0.2f;
        output = Mathf.Clamp01 (output);
        output = Mathf.Pow (output, 2f);
        return output;
    }
    private Vector2 SwitchToRectTransform (RectTransform from, RectTransform to) {
        Vector2 localPoint;
        Vector2 fromPivotDerivedOffset = new Vector2 (from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint (null, from.position);
        screenP += fromPivotDerivedOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle (to, screenP, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2 (to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
        return to.anchoredPosition + localPoint - pivotDerivedOffset;
    }
    private void HighlightUIElement (GameObject element) {
        selection.GetComponent<RectTransform> ().anchoredPosition = SwitchToRectTransform (element.GetComponent<RectTransform> (), selection.GetComponent<RectTransform> ());
        selection.GetComponent<RectTransform> ().sizeDelta = element.GetComponent<RectTransform> ().sizeDelta;
    }
}
