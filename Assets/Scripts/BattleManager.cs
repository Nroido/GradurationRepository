using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //テスト
    public List<CharacterData> testPartyData; // インスペクタで手動アサイン
    public List<CharacterData> testEnemyData;

    private void Awake()
    {
        foreach (var data in testPartyData)
            party.Add(new CharacterRuntimeState(data));

        foreach (var data in testEnemyData)
            enemies.Add(new CharacterRuntimeState(data));
        StartBattle();
    }

    //テストEND

    public List<CharacterRuntimeState> party = new List<CharacterRuntimeState>();
    public List<CharacterRuntimeState> enemies = new List<CharacterRuntimeState>();

    private bool waitingForInput = false;
    private bool attackPressed = false;

    public void StartBattle()
    {
        //戦闘開始
        StartCoroutine(PlayerTurn());
    }

    // UIの攻撃ボタンからこれを呼ぶ
    public void OnAttackButtonPressed()
    {
        if (!waitingForInput) return; // 入力待ちでない時は無視
        attackPressed = true;
    }

    private IEnumerator PlayerTurn()
    {
        waitingForInput = true;
        attackPressed = false;
        yield return new WaitUntil(() => attackPressed);
        waitingForInput = false;

        foreach (var member in party)
        {
            if (!IsAlive(member)) continue;

            var target = GetFirstAliveEnemy();
            if (target == null) break;

            var skill = member.baseData.skills[0];
            int damage = DamageCalculator.CalculateFinalDamage(member, target, skill);
            target.currentHP -= damage;

            Debug.Log($"{member.baseData.characterName} の攻撃! {target.baseData.characterName} に {damage} ダメージ (残りHP: {target.currentHP})");
        }

        if (IsBattleOver()) yield break;
        StartCoroutine(EnemyTurn());
    }

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
}