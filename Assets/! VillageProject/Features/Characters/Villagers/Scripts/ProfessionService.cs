using System;
using System.Linq;
using UnityEngine;

public class ProfessionService : MonoBehaviour
{
    [SerializeField] private Profession[] _professions;

    public Profession GetProfession(ProfessionType professionType)
    {
        return _professions.FirstOrDefault(p => p.Type == professionType);
    }
}

