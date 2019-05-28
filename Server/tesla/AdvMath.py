# 
# Advmath.py
# tesla.Advmath.py
#

import math

# -----------------------------------------------------------------------------
# Support functions

def CosLawASS(theta, b, a):
    q2=1
    q1=-2*b*math.cos(math.radians(math.fabs(theta)))
    q0=b*b-a*a
                
    """q2=1 
    q1=1 
    q0=-6""" 

    part1=-1*q1 
    part2=math.sqrt(q1*q1-4*q2*q0) 
    part3=2*q2 

    ans1=(part1+part2)/part3 
    ans2=(part1-part2)/part3 

    if ans1>0 :
        return ans1 
    else:
        return ans2 

def CosLawSAS(b, theta, c):
    return math.sqrt(b * b + c * c - 2 * b * c* math.cos(math.radians(math.fabs(theta)))) 

def CosLawSSS(a, b, c):
    return math.degrees(math.acos((b * b + c * c - a * a) / (2 * b * c))) 

def Quadratic(q2, q1, q0):
    part1=-1*q1 
    part2=math.sqrt(q1*q1-4*q2*q0) 
    part3=2*q2 

    ans1=(part1+part2)/part3 
    ans2=(part1-part2)/part3 

    if ans1>0:
        return ans1 
    else:
        return ans2 
                
def SinLawSSA(a, b, thetaA):
    if a == 0:
        return -1 
    else:
        return math.degrees(math.asin(b*math.sin(math.radians(math.fabs(thetaA))) / a)) 

def SinLawSAA(a, thetaB, thetaA):
    if math.sin(math.radians(math.fabs(thetaA))) == 0:
        return -1
    else:
        return math.sin(math.radians(thetaB))*a/math.sin(math.radians(math.fabs(thetaA))) 

def GetThetaDiff(a, b):
    thetaDiff = math.fabs(a.theta - b.theta)
    return thetaDiff

def GetCarouselDiff(a, b, a2bDeg):
    carouselDiff = math.fabs(math.fabs(a2bDeg) - math.fabs(a.carousel - b.carousel))
    return carouselDiff

def GetThetaFromR(name, r, armLength, totalDistance, RefDistance, RefTheta, VialOffsetDeg, thetaToRealDeg):
    try:
        theta = CosLawSSS( r,armLength, totalDistance)
    except:
        theta =1000000
    
    try:
        totalTheta1 = CosLawSSS(RefDistance,armLength,  totalDistance)
    except:
        totalTheta1 =0

    if theta<-99999 or 99999<theta:
        theta=0

    roboTheta = (-theta+totalTheta1+RefTheta) * thetaToRealDeg +VialOffsetDeg
    return roboTheta

def GetCarouselFromR(name,r, armLength, totalDistance, RefDistance, RefCarousel, VialOffsetDeg):
    try:
        carousel = CosLawSSS(armLength,r, totalDistance)
    except:
        carousel =1000000
    try:
        carousel2 = CosLawSSS(armLength,RefDistance,  totalDistance)
    except:
        carousel2 =0
        
    if carousel<-99999 or 99999<carousel:
        carousel=0
                
    roboCarousel = carousel-carousel2+RefCarousel +VialOffsetDeg
    return roboCarousel
        
# eof

