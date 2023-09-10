using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public static int gameSize = 2;
    public Text win, lose;
    private int GameWin=0, GameLoose=0;
    [SerializeField]
    private GameObject CardPrefab;
    [SerializeField]
    private GameObject cardList;
    [SerializeField]
    private Sprite cardBack;
    [SerializeField]
    private Sprite[] sprites;
    private Card_Manager[] cards;

    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private Card_Manager spritePreload;
    private int spriteSelected;
    private int cardSelected;
    private int cardLeft;
    private bool gameStart;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        gameStart = false;
        panel.SetActive(false);
    }
    private void PreloadCardImage()
    {
        for (int i = 0; i < sprites.Length; i++)
            spritePreload.SpriteID = i;
        spritePreload.gameObject.SetActive(false);
    }
    public void StartCardGame()
    {
        GameWin = 0; GameLoose = 0;
        win.text = GameWin.ToString(); lose.text=GameLoose.ToString();
        if (gameStart) return; 
        gameStart = true;
        
        panel.SetActive(true);
        SetGamePanel();
        cardSelected = spriteSelected = -1;
        cardLeft = cards.Length;
        SpriteCardAllocation();
        StartCoroutine(HideFace());
    }

    private void SetGamePanel(){
       

        int isOdd = gameSize % 2 ;

        cards = new Card_Manager[gameSize * gameSize - isOdd];
        foreach (Transform child in cardList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        RectTransform panelsize = panel.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        float row_size = panelsize.sizeDelta.x;
        float col_size = panelsize.sizeDelta.y;
        float scale = 1.0f/gameSize;
        float xInc = row_size/gameSize;
        float yInc = col_size/gameSize;
        float curX = -xInc * (float)(gameSize / 2);
        float curY = -yInc * (float)(gameSize / 2);

        if(isOdd == 0) {
            curX += xInc / 2;
            curY += yInc / 2;
        }
        float initialX = curX;
        for (int i = 0; i < gameSize; i++)
        {
            curX = initialX;
            for (int j = 0; j < gameSize; j++)
            {
                GameObject c;
                if (isOdd == 1 && i == (gameSize - 1) && j == (gameSize - 1))
                {
                    int index = gameSize / 2 * gameSize + gameSize / 2;
                    c = cards[index].gameObject;
                }
                else
                {
                    c = Instantiate(CardPrefab);
                    c.transform.parent = cardList.transform;

                    int index = i * gameSize + j;
                    cards[index] = c.GetComponent<Card_Manager>();
                    cards[index].ID = index;
                    c.transform.localScale = new Vector3(scale, scale);
                }
                c.transform.localPosition = new Vector3(curX, curY, 0);
                curX += xInc;

            }
            curY += yInc;
        }

    }
    void ResetFace()
    {
        for (int i = 0; i < gameSize; i++)
            cards[i].ResetRotation();
    }
    IEnumerator HideFace()
    {
        yield return new WaitForSeconds(0.0000001f);
        gameSize = Random.Range(2,7);
        for (int i = 0; i < cards.Length; i++)
            cards[i].Flip();
        yield return new WaitForSeconds(0.5f);
    }
    private void SpriteCardAllocation()
    {
        int i, j;
        int[] selectedID = new int[cards.Length / 2];
        for (i = 0; i < cards.Length/2; i++)
        {
            int value = Random.Range(0, sprites.Length - 1);
            for (j = i; j > 0; j--)
            {
                if (selectedID[j - 1] == value)
                    value = (value + 1) % sprites.Length;
            }
            selectedID[i] = value;
        }

        for (i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }
        for (i = 0; i < cards.Length / 2; i++)
            for (j = 0; j < 2; j++)
            {
                int value = Random.Range(0, cards.Length - 1);
                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedID[i];
            }

    }
    public Sprite GetSprite(int spriteId)
    {
        return sprites[spriteId];
    }
    public Sprite CardBack()
    {
        return cardBack;
    }
    public bool canClick()
    {
        if (!gameStart)
            return false;
        return true;
    }
    public void cardClicked(int spriteId, int cardId)
    {
        if (spriteSelected == -1)
        {
            spriteSelected = spriteId;
            cardSelected = cardId;
          
        }
        else
        {   if (spriteSelected == spriteId)
            {
                cards[cardSelected].Inactive();
                cards[cardId].Inactive();
                cardLeft -= 2;
                CheckGameWin();
                GameWin++;
                win.text = GameWin.ToString();
                
            }
            else
            {
                cards[cardSelected].Flip();
                cards[cardId].Flip();
                GameLoose++;
                lose.text = GameLoose.ToString();
            }
            cardSelected = spriteSelected = -1;
        }
    }
    private void CheckGameWin()
    {
        if (cardLeft == 0)
        {
            EndGame();
            Sound.Instance.PlayAudio(1);
        }
    }
    private void EndGame()
    {
        gameStart = false;
        panel.SetActive(false);
    }
    public void GiveUp()
    {
        EndGame();
    }
    
    private void Update(){
        
    }
}
