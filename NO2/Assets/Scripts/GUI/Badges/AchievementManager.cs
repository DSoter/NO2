using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [Header("Scriptable Object")]
    [SerializeField] private BadgeCollection badgeCollection;

    [Header("Pop Up")]
    [SerializeField] private Animator popUpAnimator;
    [SerializeField] private Image achievementImage;
    [SerializeField] private TextMeshProUGUI achievementName;
    [SerializeField] private TextMeshProUGUI achievementDescription;
    private Dictionary<string, Achievement> _achievements = new Dictionary<string, Achievement>();

    // Eventos por id de logro
    private Dictionary<string, Action> _singleEvents = new Dictionary<string, Action>();
    private Dictionary<string, Action<int>> _counterEvents = new Dictionary<string, Action<int>>();

    private void Awake()
    {
        LoadAll();
        RegisterCallbacks();
    }

    private void LoadAll()
    {
        foreach (Badge badge in badgeCollection.allBadges)
        {
            if (badge.achievement == null) continue;
            badge.achievement.Load();
            _achievements[badge.achievement.achievementName] = badge.achievement;
        }
    }

    private void RegisterCallbacks()
    {
        foreach (Badge badge in badgeCollection.allBadges)
        {
            if (badge.achievement == null) continue;
            Badge b = badge; // captura local
            badge.achievement.OnCompleted += () => OnAchievementCompleted(b);
            badge.achievement.OnReset += () => OnAchievementReset(b);
        }
    }

    private void OnAchievementCompleted(Badge badge)
    {
        Debug.Log($"Logro completado: {badge.achievement.achievementName} → Insignia desbloqueada: {badge.badgeName}");
        ActivatePopUp(badge, badge.achievement);
        badgeCollection.Unlock(badge);
    }
    private void OnAchievementReset(Badge badge)
    {
        Debug.Log($"Logro perdido: {badge.achievement.achievementName} → Insignia bloqueada: {badge.badgeName}");
        badgeCollection.Lock(badge);
    }

    // Llamar desde cualquier script para notificar un evento simple
    public void NotifyEvent(string achievementId)
    {
        if (!_achievements.ContainsKey(achievementId)) return;
        Achievement achievement = _achievements[achievementId];
        if (achievement.isCounter) return;
        achievement.Complete();
    }

    // Llamar desde cualquier script para incrementar un contador
    public void NotifyCounter(string achievementId, int amount = 1)
    {
        if (!_achievements.ContainsKey(achievementId)) return;
        Achievement achievement = _achievements[achievementId];
        if (!achievement.isCounter) return;
        achievement.Increment(amount);
    }

    public Achievement GetAchievement(string achievementId)
    {
        _achievements.TryGetValue(achievementId, out Achievement a);
        return a;
    }

    [ContextMenu("Reset all achievements")]
    public void ResetAll()
    {
        foreach (Achievement a in _achievements.Values)
            a.Reset();
    }
    public void ActivatePopUp(Badge badge, Achievement achievement)
    {
        achievementImage.sprite = badge.icon;
        achievementName.text = achievement.name;
        achievementDescription.text = achievement.description;
        //popUpAnimator.gameObject.SetActive(true);
        popUpAnimator.SetTrigger("start_animation");
    }
}