using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [Min(100)]
    public int ShuffleCount = 100;

    List<Card.Data> cards;

    public Card CardPrefab;
    public GameObject Dealer;
    public GameObject Player;

    private void Awake()
    {
        InitCards();//確認用のコード
    }

    void InitCards()
    {
        cards = new List<Card.Data>(13 * 4);
        var marks = new List<Card.Mark>() {
            Card.Mark.Heart,
            Card.Mark.Diamond,
            Card.Mark.Spade,
            Card.Mark.Crub,
        };

        foreach (var mark in marks)
        {
            for (var num = 2; num <= 14; ++num)
            {
                var card = new Card.Data()
                {
                    Mark = mark,
                    Number = num,
                };
                cards.Add(card);
            }
        }

        ShuffleCards();
    }

    void ShuffleCards()
    {
        //シャッフルする
        var random = new System.Random();
        for (var i = 0; i < ShuffleCount; ++i)
        {
            var index = random.Next(cards.Count);
            var index2 = random.Next(cards.Count);

            //カードの位置を入れ替える。
            var tmp = cards[index];
            cards[index] = cards[index2];
            cards[index2] = tmp;
        }
    }

    public enum Action
    {
        WaitAction=0,
        High=1,
        Low=2,
    }
    Action CurrentAction = Action.WaitAction;
    public void SetAction(int action)
    {
        CurrentAction = (Action)action;    
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            InitCards();

            
            //カードを配る
            DealCards();
            
            //プレイヤーが行動を決めるまで待つ
            bool waitAction = true;
            do
            {
                CurrentAction = Action.WaitAction;
                yield return new WaitWhile(() => CurrentAction == Action.WaitAction);
                //行う行動に合わせて処理を分岐する
                switch (CurrentAction)
                {
                    case Action.High:
                        //比較して高ければ勝ち、低ければ負け、Drowを作る
                        //裏面のカードを表にする

                        waitAction = false;
                        break;
                    case Action.Low:
                        //比較して低ければ勝ち、高ければ負け、Drowを作る
                        waitAction = false;
                        break;
                    default:
                        waitAction = true;
                        throw new System.Exception("エラー");
                }
            } while (waitAction);
            //ゲームの結果を判定する
        }
    }
    Coroutine _gameLoopCoroutine;
    private void Start()
    {
        _gameLoopCoroutine = StartCoroutine(GameLoop());
    }

    Card.Data DealCard()
    {
        if (cards.Count <= 0) return null;

        var card = cards[0];
        cards.Remove(card);
        return card;
    }

    void DealCards()
    {
        foreach (Transform card in Dealer.transform)
        {
            Object.Destroy(card.gameObject);
        }

        foreach (Transform card in Player.transform)
        {
            Object.Destroy(card.gameObject);
        }
        {
            var upCardObj = Object.Instantiate(CardPrefab, Dealer.transform);
            var upCard = DealCard();
            upCardObj.SetCard(upCard.Number, upCard.Mark, true);
        }
        {
            var cardObj = Object.Instantiate(CardPrefab, Player.transform);
            var card = DealCard();
            cardObj.SetCard(card.Number, card.Mark, false);
        }
    }

   }
