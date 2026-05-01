public class DialogueChooser
{
    public (Villager first, Villager second) GetSpeakerOrder(Villager villagerA, Villager villagerB, string dialogueID)
    {
        if (dialogueID.StartsWith("WithBlacksmith") && IsVillagerBlacksmithOrArmorer(villagerB))
        { 
            return (villagerB, villagerA);
        }

        if (dialogueID.StartsWith("WithDefender") && IsVillagerDefenderOrArcher(villagerB))
        {
            return (villagerB, villagerA);
        }
        
        return (villagerA, villagerB);
    }
    
    public string ChooseDialogueID(Villager villagerA, IDialogueTarget target, bool isShort = false)
    {
        if (target is Villager villagerB)
        {
            if (IsVillagerLowLoyalty(villagerA) && IsVillagerLowLoyalty(villagerB))
            {
                if (isShort)
                {
                    return RandomChoose("LowLoyaltyVillageShort1", "LowLoyaltyVillageShort2", "LowLoyaltyVillageShort3");
                }
                else
                {
                    return ChooseDialogueWithVillager(villagerA, villagerB,
                        new[] { "WithBlacksmithLow1", "WithBlacksmithLow2" },
                        new[] { "WithDefenderLow1", "WithDefenderLow2" },
                        new[] { "LowLoyaltyVillage1", "LowLoyaltyVillage2" });
                }
            }
            else if (IsVillagerHightLoyalty(villagerA) && IsVillagerHightLoyalty(villagerB))
            {
                if (isShort)
                {
                    return RandomChoose("HightLoyaltyVillageShort1", "HightLoyaltyVillageShort2", "HightLoyaltyVillageShort3");
                }
                else
                {
                    return ChooseDialogueWithVillager(villagerA, villagerB,
                        new[] { "WithBlacksmithHight1", "WithBlacksmithHight2" },
                        new[] { "WithDefenderHight1", "WithDefenderHight2" },
                        new[] { "HightLoyaltyVillage1", "HightLoyaltyVillage2" });
                }
            }
            else
            {
                if (isShort)
                {
                    return RandomChoose("MediumLoyaltyVillageShort1", "MediumLoyaltyVillageShort2", "MediumLoyaltyVillageShort3");
                }
                else
                {
                    return ChooseDialogueWithVillager(villagerA, villagerB,
                        new[] { "WithBlacksmithMedium1", "WithBlacksmithMedium2" },
                        new[] { "WithDefenderMedium1", "WithDefenderMedium2" },
                        new[] { "MediumLoyaltyVillage1", "MediumLoyaltyVillage2" });
                }
            }
        }

        if (target is FirstPersonController player)
        {
            if (IsVillagerLowLoyalty(villagerA))
            {
                return ChooseShortDialogueWithPlayer(villagerA, "LowLoyaltyUpgradeShort", 
                    "LowLoyaltyWithPlayerShort1", "LowLoyaltyWithPlayerShort2", "LowLoyaltyWithPlayerShort3");
            }
            else if (IsVillagerHightLoyalty(villagerA))
            {
                return ChooseShortDialogueWithPlayer(villagerA, "HightLoyaltyUpgradeShort", 
                    "HightLoyaltyWithPlayerShort1", "HightLoyaltyWithPlayerShort2", "HightLoyaltyWithPlayerShort3");
            }
            else
            {
                return ChooseShortDialogueWithPlayer(villagerA, "MediumLoyaltyUpgradeShort", 
                    "MediumLoyaltyWithPlayerShort1", "MediumLoyaltyWithPlayerShort2", "MediumLoyaltyWithPlayerShort3");
            }
        }

        if (target is EnemyInstaller enemy)
        {
            if (IsVillagerDefenderOrArcher(villagerA))
            {
                return RandomChoose("AttackShort1", "AttackShort2", "AttackShort3", "AttackShort4");
            }
            else
            {
                return RandomChoose("PeasantsInAttackShort1", "PeasantsInAttackShort2", "PeasantsInAttackShort3");
            }
        }

        return null;
    }
    
    private string ChooseDialogueWithVillager(Villager villagerA, Villager villagerB, string[] blacksmithDialogue, string[] defenderDialogue, 
        string[] baseDialogue)
    {
        bool isBlacksmithProfessionPhrase = false;
        bool isDefenderProfessionPhrase = false;
                    
        if (IsVillagerBlacksmithOrArmorer(villagerA) || IsVillagerBlacksmithOrArmorer(villagerB))
        {
            isBlacksmithProfessionPhrase = UnityEngine.Random.Range(0f, 1f) <= 0.7f;
        }

        if (IsVillagerDefenderOrArcher(villagerA) || IsVillagerDefenderOrArcher(villagerB))
        {
            isDefenderProfessionPhrase = UnityEngine.Random.Range(0f, 1f) <= 0.7f;
        }

        if (isBlacksmithProfessionPhrase && isDefenderProfessionPhrase)
        {
            isBlacksmithProfessionPhrase = UnityEngine.Random.Range(0f, 1f) <= 0.5f;
            isDefenderProfessionPhrase = !isBlacksmithProfessionPhrase;
        }

        if (isBlacksmithProfessionPhrase)
        {
            if (IsVillagerBlacksmithOrArmorer(villagerB)) (villagerB, villagerA) = (villagerA, villagerB);
            return RandomChoose(blacksmithDialogue);
        }

        if (isDefenderProfessionPhrase)
        {
            if (IsVillagerDefenderOrArcher(villagerB)) (villagerB, villagerA) = (villagerA, villagerB);
            return RandomChoose(defenderDialogue);
        }
        return RandomChoose(baseDialogue);
    }
    
    private string ChooseShortDialogueWithPlayer(Villager villagerA, string upgradeDialogue, params string[] dialogues)
    {
        bool isUpgradeDialogue = false;
        if (IsVillagerBlacksmithOrArmorer(villagerA))
        {
            isUpgradeDialogue = UnityEngine.Random.Range(0f, 1f) <= 0.7f;
        }
                    
        if (isUpgradeDialogue) return upgradeDialogue;
        return RandomChoose(dialogues);
    }

    private bool IsVillagerLowLoyalty(Villager villager)
    {
        return villager.VillagerData.Loyalty.Property.CurrentValue <= 0.3f;
    }

    private bool IsVillagerMediumLoyalty(Villager villager)
    {
        return villager.VillagerData.Loyalty.Property.CurrentValue > 0.3f && villager.VillagerData.Loyalty.Property.CurrentValue < 0.8f;
    }

    private bool IsVillagerHightLoyalty(Villager villager)
    {
        return villager.VillagerData.Loyalty.Property.CurrentValue >= 0.8f;
    }

    private bool IsVillagerBlacksmithOrArmorer(Villager villager)
    {
        return villager.VillagerData.Profession.Type == ProfessionType.Blacksmith || villager.VillagerData.Profession.Type == ProfessionType.Armorer;;
    }

    private bool IsVillagerDefenderOrArcher(Villager villager)
    {
        return villager.VillagerData.Profession.Type == ProfessionType.Archer || villager.VillagerData.Profession.Type == ProfessionType.Defender;
    }

    private string RandomChoose(params string[] choices)
    {
        return choices[UnityEngine.Random.Range(0, choices.Length)];
    }
}