# 
# AxisTracker.py
# tesla.hardware.AxisTracker
#
# Keeps track of the usage of the axis
# 
# Copyright (c) Invetech Operations Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Operations Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Operations Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
# 

from datetime import date
import anydbm
        
# ---------------------------------------------------------------------------
def Report(file = None):
    """ Displays the current axis tracker database """
    if file == None:
        dbFile = AxisTracker.dbFilePath
    else:
        dbFile = file
        
    db = anydbm.open (dbFile, 'r')

    try:
        print "%-16s %-17s %-10s %17s %15s %15s" % ("Name:", "Commissioned:", "Movements:", "Steps (x10^6):", "Full motions:", "Failures/Homes:")

        for axisName in db.keys():
            #(maxStep, totalMovements, totalMegaSteps, totalSteps, ordinalDate) = db[axisName].split(',')
            row = _ObtainTrackerData (db, axisName)
            commissionDate = date.fromordinal (int (row[AxisTracker.CommissionDateField])).strftime ("%d %b, %Y")
            fullTraverses = _FullTraverses (int(row[AxisTracker.MaxStepField]), int(row[AxisTracker.TotalMegaStepsField]), int(row[AxisTracker.TotalStepsField]))
            print "%-16s %-17s %10s %10s.%06d %15d %6s/%s" % \
                  (axisName, \
                   commissionDate, \
                   row[AxisTracker.TotalMovementsField], \
                   row[AxisTracker.TotalMegaStepsField], \
                   int(row[AxisTracker.TotalStepsField]), \
                   fullTraverses, \
                   row[AxisTracker.TotalHomeFailsField], \
                   row[AxisTracker.TotalHomesField])

    finally:
        db.close()

# ---------------------------------------------------------------------------
def _ObtainTrackerData(db, key):
    """Obtain data for the given Axis key.
    Return a list of values, padded out if the number of fields expected > number found"""
    data = db[key].split(',')
    data += (AxisTracker.FieldCount - len(data))*['0',]
    return data                    

# ---------------------------------------------------------------------------
def _FullTraverses(maxStep, nbrMegaSteps, nbrSteps):
    """Return the number of full traverses. (ie. the number of steps moved divided by the maximum traverse.)"""
    return int ((float(nbrMegaSteps)/maxStep) * AxisTracker.stepsPerMegaStep + nbrSteps/maxStep)       

# ---------------------------------------------------------------------------
def _Kilosteps(  nbrMegaSteps, nbrSteps ):
    """Return the number of kilosteps."""
    return  ( float( nbrMegaSteps ) * AxisTracker.stepsPerMegaStep + float( nbrSteps ) ) / 1000.      

# ---------------------------------------------------------------------------

class AxisTracker:
    """Class to record usage of a stepper card axis"""
    stepsPerMegaStep = 1000000
    dbFilePath = ".\AxisTracker.dbm"  # A default value, override at startup, by all means!

    # FieldIndices:
    #
    MaxStepField        = 0
    TotalMovementsField = 1
    TotalMegaStepsField = 2
    TotalStepsField     = 3
    CommissionDateField = 4
    TotalHomeFailsField = 5
    TotalHomesField     = 6
    FieldCount     = 7


    #-------------------------------------------------------------------------
    def __init__ (self, name, maxStep):
        """Load initial settings"""
        self.name = name
        self.maxStep = maxStep        
        self.totalMovements = 0
        self.totalMegaSteps = 0        
        self.totalSteps = 0
        self.totalHomes = 0
        self.totalHomeFails = 0
        
        self.__LoadData ()

    #-------------------------------------------------------------------------
    
    def NoteSteps (self, stepIncrement):
        """Note steps, and movement"""
        self.__NoteSteps (stepIncrement)

        # Update the database
        #
        self.__StoreData()

    #-------------------------------------------------------------------------
    def NoteHomeAction (self, stepIncrement, homeOK):
        """Note each home, and whether it failed."""
        self.totalHomes +=1
        if not homeOK:
            self.totalHomeFails +=1
            
        self.__NoteSteps (stepIncrement)
        
        # Update the database
        #
        self.__StoreData()
        
    #-------------------------------------------------------------------------
    def NbrFullTraverses (self):
        """Return the number of full traverses. (ie. the number of steps moved divided by the maximum traverse.)"""
        return _FullTraverses (self.maxStep, self.totalMegaSteps, self.totalSteps)       

    def NbrKilosteps( self ):
        """Return the number of kilosteps."""
        return _Kilosteps( self.totalMegaSteps, self.totalSteps )


    #-------------------------------------------------------------------------
    def __NoteSteps (self, stepIncrement):
        """Increment number of steps, and overflow into megasteps"""
        iStepIncrement = abs(stepIncrement)
        self.totalSteps += iStepIncrement

        # Adjust for overflow
        #
        self.totalMegaSteps += int (self.totalSteps/AxisTracker.stepsPerMegaStep)        
        self.totalSteps = self.totalSteps % AxisTracker.stepsPerMegaStep
        
        self.totalMovements += 1        

    #-------------------------------------------------------------------------
    def __LoadData (self):
        """Load data from the database, creating the file if not already present"""

        isNew = True
        db = anydbm.open (AxisTracker.dbFilePath, 'c')

        try:
            if self.name in db.keys():
                row = _ObtainTrackerData(db, self.name)
                self.maxStep        = int (row[0])
                self.totalMovements = int (row[1])
                self.totalMegaSteps = int (row[2])
                self.totalSteps     = int (row[3])
                self.commissionDate = int (row[4])
                self.totalHomeFails = int (row[5])
                self.totalHomes     = int (row[6])
                   
                isNew = False

            else:
                # Note the date as the commission date for the component.
                #
                self.commissionDate = date.today().toordinal()

        finally:
            db.close()

        if isNew:
            # Add new entry to database
            self.__StoreData()

    #-------------------------------------------------------------------------
    def __StoreData (self):
        """Store data to the database"""
        db = anydbm.open (AxisTracker.dbFilePath, 'w')

        try:
            dataString = "%d, %d, %d, %d, %d, %d, %d" % \
                            (
                            self.maxStep,
                            self.totalMovements,
                            self.totalMegaSteps,
                            self.totalSteps,
                            self.commissionDate,
                            self.totalHomeFails,
                            self.totalHomes,
                            )
            db[self.name] = dataString

        finally:
            db.close()

# EOF

