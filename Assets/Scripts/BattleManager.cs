using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //テスト
    public List<CharacterData> testPartyData; 
    public List<CharacterData> testEnemyData;

    private List<CardData> currentHand;
    private CardData selectedCard;
    private CharacterRuntimeState currentActor;
    private void Awake()
    {
        foreach (var data in testPartyData)
            party.Add(new CharacterRuntimeState(data));

        foreach (var data in testEnemyData)
            enemies.Add(new CharacterRuntimeState(data));
        StartBattle();
    }

    private void Update()
    {

    }

    //テストEND
    public List<CardData> sharedGenericPool;


    //コスト
    public int currentPartyCost;
    public int maxPartyCost = 5;

    private void ResetPartyCost()
    {
        currentPartyCost = maxPartyCost;
        Debug.Log($"コストリセット: {currentPartyCost}/{maxPartyCost}");
    }

    public bool TrySpendCost(int cost)
    {
        if (currentPartyCost < cost)
        {
            Debug.Log("コストが足りません!");
            return false;
        }
        currentPartyCost -= cost;
        Debug.Log($"コスト消費: -{cost} (残り {currentPartyCost}/{maxPartyCost})");
        return true;
    }
    //コストエンド

    public List<CharacterRuntimeState> party = new List<CharacterRuntimeState>();
    public List<CharacterRuntimeState> enemies = new List<CharacterRuntimeState>();

    

    private bool waitingForInput = false;
    private bool attackPressed = false;
    private CharacterRuntimeState currentActingMember;

    public void StartBattle()
    {
        //戦闘開始
        StartCoroutine(PlayerTurn());
    }

    // UIの攻撃ボタンからこれを呼ぶ
    public void OnAttackButtonPressed()
    {
        if (!waitingForInput) return; 
        attackPressed = true;
    }

    public void OnAbilityButtonPressed(int slotIndex)
    {
        if (!waitingForInput) return;
        if (slotIndex < 0 || slotIndex >= party.Count) return;

        var user = party[slotIndex]; 
        var skill = user.baseData.skills.Find(s => s.skillType == SkillType.Ability);
        if (skill == null) return;
        if (!user.IsSkillReady(skill)) return;//クールタイム中

        ExecuteSkill(user, skill);
        user.UseSkill(skill);
    }

    private void ExecuteSkill(CharacterRuntimeState user, SkillData skill)
    {
        //ダメージテスト用
        var target = GetFirstAliveEnemy();
        if (target == null) return;

        int damage = DamageCalculator.CalculateFinalDamage(user, target, skill);
        target.currentHP -= damage;
        Debug.Log($"{user.baseData.characterName} の {skill.skillName}! {target.baseData.characterName} に {damage} ダメージ (残りHP: {target.currentHP})");
    }

    private IEnumerator PlayerTurn()
    {
        Debug.Log("=== 味方ターン開始 ===");
        ResetPartyCost();
        foreach (var member in party)
        {
            if (!IsAlive(member)) continue;

            currentActor = member;
            currentHand = GenerateHand(member);

            Debug.Log($"--- {member.baseData.characterName} の手番 (残りコスト {currentPartyCost}/{maxPartyCost}) ---");
            for (int i = 0; i < currentHand.Count; i++)
            {
                Debug.Log($"[{i}] {currentHand[i].cardName} (コスト{currentHand[i].cost})");
            }

            selectedCard = null;
            yield return new WaitUntil(() => selectedCard != null);

            ExecuteCard(member, selectedCard);
        }

        if (IsBattleOver()) yield break;
        StartCoroutine(EnemyTurn());
    }

    // UIのカードボタンから呼ぶ(引数はカードのインデックス0~5)
    public void OnCardSelected(int handIndex)
    {
        if (handIndex < 0 || handIndex >= currentHand.Count) return;

        var card = currentHand[handIndex];
        if (card.cost > 0 && currentPartyCost < card.cost)
        {
            Debug.Log("コストが足りません!");
            return; // selectedCardをセットしないので入力待ちのまま
        }

        selectedCard = card;
    }

    //ーーーーーーーーーーーーーーーーーーーーーーーカード効果の実装
    private void ExecuteCard(CharacterRuntimeState user, CardData card)
    {
        if (card.cost > 0)
        {
            TrySpendCost(card.cost);
        }

        switch (card.category)
        {
            case CardCategory.NormalAttack:
            case CardCategory.CharacterSkill:
            case CardCategory.Ultimate:
                var target = GetFirstAliveEnemy();
                if (target == null) return;
                int damage = DamageCalculator.CalculateFinalDamage(user, target, card.linkedSkill);
                target.currentHP -= damage;
                Debug.Log($"{user.baseData.characterName} の {card.cardName}! {target.baseData.characterName} に {damage} ダメージ (残りHP: {target.currentHP})");
                break;

            case CardCategory.Generic:
                Debug.Log($"{user.baseData.characterName} が {card.cardName} を使用(汎用カード効果は未実装)");
                // 汎用カードは使い切りなので、プールから除外する処理を後で追加
                break;
        }
    }

    //ーーーーーーーーーーーーーーーーーーーーーーーカード効果の実装END

    private IEnumerator EnemyTurn()
    {
        Debug.Log("--- 敵ターン ---");
        yield return new WaitForSeconds(1f); // 演出待ちの仮ウェイト

        foreach (var enemy in enemies)
        {
            if (!IsAlive(enemy)) continue;

            var target = GetFirstAliveMember();
            if (target == null) break;

            var skill = enemy.baseData.skills[0];
            int damage = DamageCalculator.CalculateFinalDamage(enemy, target, skill);
            target.currentHP -= damage;

            Debug.Log($"{enemy.baseData.characterName} の攻撃! {target.baseData.characterName} に {damage} ダメージ (残りHP: {target.currentHP})");
        }

        if (IsBattleOver()) yield break;
        StartCoroutine(PlayerTurn());
    }

    private bool IsAlive(CharacterRuntimeState c) => c.currentHP > 0;

    private CharacterRuntimeState GetFirstAliveEnemy()
    {
        foreach (var e in enemies) if (IsAlive(e)) return e;
        return null;
    }

    private CharacterRuntimeState GetFirstAliveMember()
    {
        foreach (var m in party) if (IsAlive(m)) return m;
        return null;
    }

    private bool IsBattleOver()
    {
        if (GetFirstAliveEnemy() == null) { Debug.Log("=== 勝利! ==="); return true; }
        if (GetFirstAliveMember() == null) { Debug.Log("=== 敗北... ==="); return true; }
        return false;
    }

    public List<CardData> GenerateHand(CharacterRuntimeState character)
    {
        var hand = new List<CardData>();

        // 1. 通常攻撃カードを固定で1枚追加
        var normalAttackCard = character.baseData.ownedCards
            .FirstOrDefault(c => c.category == CardCategory.NormalAttack);
        if (normalAttackCard != null)
        {
            hand.Add(normalAttackCard);
        }
        else
        {
            Debug.LogWarning($"{character.baseData.characterName} に通常攻撃カードが設定されていません");
        }

        // 2. 抽選対象プールを作る(専用カードのうち通常攻撃以外 + 汎用共有プール)
        var pool = new List<CardData>();
        pool.AddRange(character.baseData.ownedCards.Where(c => c.category != CardCategory.NormalAttack));
        pool.AddRange(sharedGenericPool.Where(c => c.category != CardCategory.NormalAttack));

        // 3. ランダムに5枚引く(重複可・今は単純にランダム選択)
        for (int i = 0; i < 5; i++)
        {
            if (pool.Count == 0) break;

            int index = Random.Range(0, pool.Count);
            hand.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return hand;
    }

}