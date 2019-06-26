# File: S (Python 2.3)

import ctypes

def _swig_setattr(self, class_type, name, value):
    if name == 'this':
        if isinstance(value, class_type):
            self.__dict__[name] = value.this
            if hasattr(value, 'thisown'):
                self.__dict__['thisown'] = value.thisown
            
            del value.thisown
            return None
        
    
    method = class_type.__swig_setmethods__.get(name, None)
    if method:
        return method(self, value)
    
    self.__dict__[name] = value


def _swig_getattr(self, class_type, name):
    method = class_type.__swig_getmethods__.get(name, None)
    if method:
        return method(self)
    
    raise AttributeError(name)

import types

try:
    _object = object
    _newclass = 1
except AttributeError:
    
    class _object:
        pass

    _newclass = 0

del types

class TimeBlock(_object):
    __swig_setmethods__ = { }
    
    __setattr__ = lambda self, name, value: _swig_setattr(self, TimeBlock, name, value)
    __swig_getmethods__ = { }
    
    __getattr__ = lambda self, name: _swig_getattr(self, TimeBlock, name)
    
    def __repr__(self):
        return '<C TimeBlock instance at %s>' % (self.this,)
    """
    __swig_setmethods__['m_OpenPeriod'] = _SchedulerPy.TimeBlock_m_OpenPeriod_set
    __swig_getmethods__['m_OpenPeriod'] = _SchedulerPy.TimeBlock_m_OpenPeriod_get
    if _newclass:
        m_OpenPeriod = property(_SchedulerPy.TimeBlock_m_OpenPeriod_get, _SchedulerPy.TimeBlock_m_OpenPeriod_set)
    
    __swig_setmethods__['m_UsedPeriod'] = _SchedulerPy.TimeBlock_m_UsedPeriod_set
    __swig_getmethods__['m_UsedPeriod'] = _SchedulerPy.TimeBlock_m_UsedPeriod_get
    if _newclass:
        m_UsedPeriod = property(_SchedulerPy.TimeBlock_m_UsedPeriod_get, _SchedulerPy.TimeBlock_m_UsedPeriod_set)
    
    __swig_setmethods__['m_FreePeriod'] = _SchedulerPy.TimeBlock_m_FreePeriod_set
    __swig_getmethods__['m_FreePeriod'] = _SchedulerPy.TimeBlock_m_FreePeriod_get
    if _newclass:
        m_FreePeriod = property(_SchedulerPy.TimeBlock_m_FreePeriod_get, _SchedulerPy.TimeBlock_m_FreePeriod_set)
    
    __swig_setmethods__['m_StartTime'] = _SchedulerPy.TimeBlock_m_StartTime_set
    __swig_getmethods__['m_StartTime'] = _SchedulerPy.TimeBlock_m_StartTime_get
    if _newclass:
        m_StartTime = property(_SchedulerPy.TimeBlock_m_StartTime_get, _SchedulerPy.TimeBlock_m_StartTime_set)
    """
    
    def __init__(self, *args):
        #_swig_setattr(self, TimeBlock, 'this', _SchedulerPy.new_TimeBlock(*args))
        _swig_setattr(self, TimeBlock, 'thisown', 1)

    
    def __del__(self, destroy):# = _SchedulerPy.delete_TimeBlock):
        
        try:
            if self.thisown:
                destroy(self)
        except:
            pass




class TimeBlockPtr(TimeBlock):
    
    def __init__(self, this):
        _swig_setattr(self, TimeBlock, 'this', this)
        if not hasattr(self, 'thisown'):
            _swig_setattr(self, TimeBlock, 'thisown', 0)
        
        _swig_setattr(self, TimeBlock, self.__class__, TimeBlock)


#_SchedulerPy.TimeBlock_swigregister(TimeBlockPtr)

class Scheduler(_object):
    __swig_setmethods__ = { }
    
    __setattr__ = lambda self, name, value: _swig_setattr(self, Scheduler, name, value)
    __swig_getmethods__ = { }
    
    __getattr__ = lambda self, name: _swig_getattr(self, Scheduler, name)
    
    def __repr__(self):
        return '<C Scheduler instance at %s>' % (self.this,)

    
    def __init__(self, *args):
        #_swig_setattr(self, Scheduler, 'this', _SchedulerPy.new_Scheduler(*args))
        _swig_setattr(self, Scheduler, 'thisown', 1)

    
    def __del__(self, destroy):# = _SchedulerPy.delete_Scheduler):
        
        try:
            if self.thisown:
                destroy(self)
        except:
            pass


    
    def SetDelayFor(*args):
        pass #return _SchedulerPy.Scheduler_SetDelayFor(*args)

    
    def DelayFor(*args):
        pass #return _SchedulerPy.Scheduler_DelayFor(*args)

    
    def AppendBlock(*args):
        pass #return _SchedulerPy.Scheduler_AppendBlock(*args)

    
    def CalculateTimes(*args):
        pass #return _SchedulerPy.Scheduler_CalculateTimes(*args)

    
    def GetBlock(*args):
        pass #return _SchedulerPy.Scheduler_GetBlock(*args)

    
    def Reset(*args):
        pass #return _SchedulerPy.Scheduler_Reset(*args)

    
    def MaxIterations(*args):
        pass #return _SchedulerPy.Scheduler_MaxIterations(*args)

    
    def SetMaxIterations(*args):
        pass #return _SchedulerPy.Scheduler_SetMaxIterations(*args)

    
    def NbrSchedules(*args):
        pass #return _SchedulerPy.Scheduler_NbrSchedules(*args)

    
    def SetNbrSchedules(*args):
        pass #return _SchedulerPy.Scheduler_SetNbrSchedules(*args)

    
    def NbrIterations(*args):
        pass #return _SchedulerPy.Scheduler_NbrIterations(*args)



class SchedulerPtr(Scheduler):
    
    def __init__(self, this):
        _swig_setattr(self, Scheduler, 'this', this)
        if not hasattr(self, 'thisown'):
            _swig_setattr(self, Scheduler, 'thisown', 0)
        
        _swig_setattr(self, Scheduler, self.__class__, Scheduler)

#_SchedulerPy.Scheduler_swigregister(SchedulerPtr)
