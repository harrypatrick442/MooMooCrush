using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace Assets.Scripts
{
    public class FighterController : IFighterController, ILooper2Hz, IGetIsActing
    {
        private float _MinEngageTime = 3;
        private float _SwitchFightingWithProbability = 10;
        private Health _Strength = new Health();
        private IFightable _CurrentlyEngaging;
        public IFightable CurrentlyEngaging
        {
            set
            {
                if (_CurrentlyEngaging != value)
                {
                    IFightable c = _CurrentlyEngaging;
                    _CurrentlyEngaging = null;
                    //both released first.
                    if (c != null)
                    {
                        c.FighterController.ReleaseCurrentlyEngaging();
                    }
                    if (value != null)
                    {
                        _CurrentlyEngaging = value;
                        value.FighterController.TakeCurrentlyEngaging(_IFightableMe);
                    }
                }
            }
            get
            {
                return _CurrentlyEngaging;
            }
        }
        public void ReleaseCurrentlyEngaging()
        {
            CurrentlyEngaging = null;
        }
        public void TakeCurrentlyEngaging(IFightable currentlyEngaging)
        {
            CurrentlyEngaging = currentlyEngaging;
        }
        private IGetCloseTo _IGetCloseTo;
        private HealthBar _HealthBar;
        private IFightable _IFightableMe;
        private IFightsController _IFightsController;
        private IResourceHelper _IResourceHelper;
        private WeakCollection<FightAction> _WeakCollectionFightingOwners = new WeakCollection<FightAction>();
        private SurroundingsScanner<IFightable> _FightableScanner;
        public bool IsBeingAttacked
        {
            get
            {
                return _WeakCollectionFightingOwners.Count > 0;
            }
        }
        private float _CurrentlyEngagedStartTime;
        private float CurrentlyEngagedDuration
        {
            get
            {
                return Time.time - _CurrentlyEngagedStartTime;
            }
        }
        private FightAction _FightAction;
        public bool IsActing
        {
            get
            {
                return _FightAction != null;
            }
        }
        public float? DistanceFromEnemy
        {
            get
            {
                if (CurrentlyEngaging != null)
                {
                    Vector2? positionA = ((IFightable)CurrentlyEngaging).GetPosition2();
                    if (positionA != null)
                    {
                        Vector2? positionB = _IFightableMe.GetPosition2();
                        if (positionB != null)
                        {
                            return ((Vector2)positionA - ((Vector2)positionB)).magnitude;
                        }
                    }
                }
                return null;
            }
        }
        private void EndedActingCallback()
        {
            _FightAction = null;
        }
        private ILooper _ILooper;
        private EventDictionary<IFightable, FightingInfo>.DictionaryUpdateHandler _DictionaryUpdateHandler;
        private EventDictionary<IFightable, FightingInfo> _FightingWith;
        public FighterController(HealthBar healthBar, IFightsController iFightsController, IFightable iFightableMe, IGetCloseTo iGetCloseTo, IResourceHelper iResourceHelper, Transform transform, float minEngageTime, float switchFightingWithProbability)
        {
            _IFightsController = iFightsController;
            _IGetCloseTo = iGetCloseTo;
            _IFightableMe = iFightableMe;
            _MinEngageTime = minEngageTime;
            _SwitchFightingWithProbability = switchFightingWithProbability;
            _HealthBar = healthBar;
            _HealthBar.gameObject.SetActive(true);
            _IResourceHelper = iResourceHelper;
            _ILooper = iResourceHelper.Get<ILooper>();
            _FightableScanner = new SurroundingsScanner<IFightable>(5, 8, transform, 0);
            _FightingWith = new EventDictionary<IFightable, FightingInfo>();
            _DictionaryUpdateHandler = new EventDictionary<IFightable, FightingInfo>.DictionaryUpdateHandler(FightingWithChanged);
            _FightingWith.AddEventHandler(_DictionaryUpdateHandler);
            _ILooper.Add2Hz(this);
        }
        private void FightingWithChanged(DictionaryEventArgs<IFightable, FightingInfo> e)
        {
            switch (e.UpdateType)
            {
                case DictionaryEventArgs<IFightable, FightingInfo>.UpdateTypes.Add:
                    break;
                case DictionaryEventArgs<IFightable, FightingInfo>.UpdateTypes.Remove:
                    if (CurrentlyEngaging != null)
                    {
                        if (e.Key.Equals(CurrentlyEngaging))
                        {
                            CurrentlyEngaging = null;
                        }
                    }
                    break;
                case DictionaryEventArgs<IFightable, FightingInfo>.UpdateTypes.Clear:
                    break;
            }
        }
        public void TakeFighting(FightAction owner)
        {
            _WeakCollectionFightingOwners.Add(owner);
        }
        public void ReleaseFighting(FightAction owner)
        {
            if (_WeakCollectionFightingOwners.Contains(owner))
                _WeakCollectionFightingOwners.Remove(owner);
        }
        private void Scan()
        {
            List<IFightable> list = _FightableScanner.Scan(5);
            if (list.Contains(_IFightableMe))
                list.Remove(_IFightableMe);
            foreach (IFightable iFightable in list)
            {
                if (!_FightingWith.Keys.Contains(iFightable))
                {
                    _FightingWith.Add(iFightable, new FightingInfo());
                }
            }
            List<IFightable> toRemove = new List<IFightable>();
            foreach (IFightable iFightable in _FightingWith.Keys)
            {
                if (!list.Contains(iFightable))
                {
                    toRemove.Add(iFightable);
                }
            }
            foreach (IFightable iFightable in toRemove)
            {
                FightingInfo fightingInfo = _FightingWith[iFightable];
                _FightingWith.Remove(iFightable);
            }
        }
        private void SwitchFightingWith()
        {
            if (_FightingWith.Count > 0)
            {
                List<IFightable> fightables = (from a in _FightingWith.Keys where !a.FighterController.IsActing select a).ToList();
                if (fightables.Count > 0)
                {
                    SetCurrentlyEngaged(fightables[Random.Range(0, fightables.Count)]);
                }
            }
        }
        private void SetCurrentlyEngaged(IFightable iFightable)
        {
            CurrentlyEngaging = iFightable;
            _CurrentlyEngagedStartTime = Time.time;
            Vector2? pos = iFightable.GetPosition2();
            if (pos != null)
            {
                _IGetCloseTo.GetCloseTo((Vector2)pos);
            }

        }
        public FightAction Attack(Enums.Attacks attack)
        {
            List<IDefence> iDefences = _IFightableMe.ViolenceHandler.GetIDefences(attack);
            if (iDefences.Count > 0)
            {
                IDefence iDefence = iDefences[Random.Range(0, iDefences.Count)];
                _FightAction = new FightAction(this, iDefence, EndedActingCallback);
                return _FightAction;
            }
            return null;
        }
        private void LaunchRandomAttack(List<Enums.Attacks> suitableAttacks)
        {
            if (suitableAttacks.Count > 0)
            {
                UnityEngine.Random.InitState(new DateTime().Second);
                int index = Random.Range((int)0, (int)suitableAttacks.Count);
                Enums.Attacks attack = suitableAttacks[index];
                FightAction fightActionEnemy = CurrentlyEngaging.FighterController.Attack(attack);
                IAttack iAttack = _IFightableMe.ViolenceHandler.GetIAttack(attack);
                _FightAction = new FightAction(this, iAttack, EndedActingCallback);
                _IFightsController.Fight(_FightAction, fightActionEnemy, _IResourceHelper);
            }
        }
        public bool Looper2Hz()
        {
            Scan();
            if (!IsActing)
            {
                if (Decisions.SwitchFightingWith(CurrentlyEngaging, CurrentlyEngaging != null ? _FightingWith[CurrentlyEngaging] : null, CurrentlyEngagedDuration, this, _MinEngageTime, _SwitchFightingWithProbability))
                {
                    SwitchFightingWith();
                }
                if (CurrentlyEngaging != null)
                {
                    List<Enums.Attacks> mutalAttacks = (from a in CurrentlyEngaging.ViolenceHandler.ReceiveAttacks where _IFightableMe.ViolenceHandler.SendAttacks.Contains(a) select a).ToList();
                    if (Decisions.IsInAttackRange(mutalAttacks, CurrentlyEngaging, _IFightableMe))
                    {
                        LaunchRandomAttack(mutalAttacks);
                    }
                }
            }
            return false;
        }
        public void Dispose()
        {
            _ILooper.Remove2Hz(this);
            ReleaseCurrentlyEngaging();
        }

        private class Decisions
        {
            public static bool SwitchFightingWith(IFightable iFightable, FightingInfo fightingInfo, float currentlyEngagedDuration, IGetIsActing iIsActing, float minEngageTime, float switchFightingWithProbability)
            {
                if (iFightable == null)
                    return true;
                if (currentlyEngagedDuration < minEngageTime || iIsActing.IsActing)
                    return false;
                return new RouletteWheel<bool>(new RouletteSlot<bool>[] { new RouletteSlot<bool>(switchFightingWithProbability, true), new RouletteSlot<bool>(100 - switchFightingWithProbability, false) }).Spin();
            }
            public static bool IsInAttackRange(List<Enums.Attacks> attacks, IFightable iFightableEnemy, IFightable iFightableMe)
            {
                Vector2? distance = iFightableEnemy.GetPosition2() - iFightableMe.GetPosition2();
                if (distance != null)
                {
                    float distanceMagnitude = ((Vector2)distance).magnitude;
                    List<Enums.Attacks> toRemove = new List<Enums.Attacks>();
                    foreach (Enums.Attacks attack in attacks)
                    {
                        var iAttack = iFightableMe.ViolenceHandler.GetIAttack(attack);
                        if (iAttack.Range < distanceMagnitude)
                        {

                            Debug.Log("removing:" + attack);
                            toRemove.Add(attack);
                        }
                    }
                    foreach (Enums.Attacks attack in toRemove)
                        attacks.Remove(attack);
                    return attacks.Count > 0;
                }
                return false;
            }
        }
    }
}