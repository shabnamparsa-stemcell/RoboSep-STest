# 
# ReferencePoint.py
# tesla.types.ReferencePoint

from tesla.exception import TeslaException

# -----------------------------------------------------------------------------

class ReferencePointException(TeslaException):
    pass

# -----------------------------------------------------------------------------

class ReferencePoint:

    """carouselOffset = 0 #-14.6
    carouselOffset2 = 0 #-8.65

    armLength = 140# 149.098
    
    samplevialDistance = 115.5 #115.5 
    separationvialDistance = 85 #85.21419695806998 
    wastevialDistance = 0 #62.79242870345972 
    lysisbuffervialDistance = 94.838 #94.3575470579595 
    cocktailvialDistance = 118.75 #118.89437807390827 
    particlevialDistance = 118.75 #118.89437807390827 
    antibodyvialDistance = 118.75 #118.89437807390827 
    position1Distance = 96.61 #96.8029287410194 
    position2Distance = 96.61 #97.07719105515598 
    position3Distance = 96.61 #96.8029287410194 
    position4Distance = 113.50 #113.54771633725386 
    position5Distance = 113.50 #113.54771633725386 


    samplevialToSampleDeg = 0
    separationvialToSampleDeg = -20.71
    wastevialToSampleDeg = 24.03
    lysisbuffervialToSampleDeg = 37.82
    cocktailvialToSampleDeg = 40
    particlevialToSampleDeg = 47.24
    antibodyvialToSampleDeg = 32.76
    position1ToSampleDeg = 22.53 #23.56 #22.76
    position2ToSampleDeg = 17.13 #18.16 #17.17
    position3ToSampleDeg = 11.72 #12.75 #11.78
    position4ToSampleDeg = 21.67 #22.71 #21.76
    position5ToSampleDeg = 12.58 #13.61 #12.68"""
	
    def __init__(self, theta, carousel, r, z):
	self.theta = theta
	self.carousel = carousel
	self.r = r
	self.z = z

    def __str__(self):
	'''A string representation of the tube'''
	return "%f %f %f %f" % (self.theta, \
                self.carousel, self.r, self.z)

   

# eof

