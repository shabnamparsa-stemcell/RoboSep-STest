# 
# VolumeProfile.py
# tesla.types.VolumeProfile
#
# Provides volume height variations for various tube cross-sections
# 
# Copyright (c) Invetech Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
# 

import math
from tesla.exception import TeslaException

# -----------------------------------------------------------------------------

class VolumeProfileError(TeslaException):
    pass

# -----------------------------------------------------------------------------

class VolumeProfile:
    """A base class for providing the height of a volume of fluid in a particularly shaped container"""
    
    # -----------------------------------------------------------------------------
    def __init__(self, fullHeight, fullVolume, baseIsSmaller = True):
        """ Define a tube geometry:
                fullHeight - the maximum height covered by the section
                fullVolume - the volume corresponding to maxHeight
                baseIsSmaller - default is 'True'. Indicates how profile fills 
        """
        #print "$$$$$$ the height is %f" % fullHeight
        #print "$$$$$$ the volume is %f" % fullVolume
        self.__m_FullHeight = float(fullHeight)
        self.__m_FullVolume = float(fullVolume)
        self.__m_BaseIsSmaller = baseIsSmaller

    # -----------------------------------------------------------------------------
    def HeightOf (self, volume):
        """Given a volume of fluid, determine the height of the meniscus.
            The relative orientation is taken into account"""
        if volume > self.FullVolume():
            raise VolumeProfileError ("Given volume (%dul) exceeds profile capacity (%dul))" % (volume, self.FullVolume()))

        if self.__m_BaseIsSmaller:
            return self._HeightOf(volume)
        else:
            return self.FullHeight() - self._HeightOf (self.FullVolume() - volume)

    # -----------------------------------------------------------------------------
    def FullHeight (self):
        """Obtain the full height of the profile"""
        return self.__m_FullHeight

    # -----------------------------------------------------------------------------
    def FullVolume (self):
        """Obtain the full volume of the profile"""
        return self.__m_FullVolume

    # -----------------------------------------------------------------------------
    def _HeightOf (self, volume):
        """Abstract method: given a volume of fluid, determine the height of the meniscus.
            Override this method according to profile"""
        pass

# -----------------------------------------------------------------------------

class DeadVolumeProfile (VolumeProfile):
    """Defines a dead volume, for which the effective height is zero."""
    
    # -----------------------------------------------------------------------------
    def __init__(self, deadVolume):
        """ Define a dead volume, which cannot be accessed by the probe:
                deadVolume - the volume which cannot be accessed.
        """
        VolumeProfile.__init__ (self, 0, deadVolume)

    # -----------------------------------------------------------------------------
    def _HeightOf (self, volume):
        """The effective height of the dead volume is always zero"""
        return 0

# -----------------------------------------------------------------------------

class CylindricalProfile (VolumeProfile):
    """Providing the height of a volume of fluid in a cylindrical section.
        NB: Since the profile is symmetric, no option to invert the profile is necessary.
    """
    
    # -----------------------------------------------------------------------------
    def __init__(self, fullHeight, fullVolume):
        """ Define a cylinder:
                fullHeight - the maximum height covered by the section (H)
                fullVolume - the volume corresponding to maxHeight (V = v(H)).
                
            In general h = Kv, where K = H/V.
        """
        VolumeProfile.__init__ (self, fullHeight, fullVolume)

        self.__m_K = self.FullHeight()/self.FullVolume()
        
    # -----------------------------------------------------------------------------
    def _HeightOf (self, volume):
        """Determine the height of the meniscus from h = kH."""
        return self.__m_K * volume

# -----------------------------------------------------------------------------

class ConicalProfile (VolumeProfile):
    """Providing the height of a volume of fluid in a conical section.
        (It is assumed the apex is at the bottom)"""

    power = 1.0/3.0    
    
    # -----------------------------------------------------------------------------
    def __init__(self, fullHeight, fullVolume, baseIsSmaller = True):
        """ Define a cone:
                fullHeight - the maximum height covered by the section (H)
                fullVolume - the volume corresponding to maxHeight (V = v(H)).
                
            In general h = K(v^1/3), where K = H/(V^1/3).
        """
        VolumeProfile.__init__ (self, fullHeight, fullVolume, baseIsSmaller)
        self.__m_K = fullHeight/(fullVolume**ConicalProfile.power)

    # -----------------------------------------------------------------------------
    def _HeightOf (self, volume):
        """Determine the height of the meniscus from h = kH."""
        return self.__m_K * (volume**ConicalProfile.power)

# -----------------------------------------------------------------------------

class TruncatedConicalProfile (ConicalProfile):
    """Providing the height of a volume of fluid in a truncated conical section."""
    
    # -----------------------------------------------------------------------------
    def __init__(self, fullHeight, fullVolume, tip, baseIsSmaller = True):
        """ Define a cone:
                fullHeight - the maximum height covered by the section (H)
                fullVolume - the volume corresponding to maxHeight (V = v(H)).
                tip - a conical profile representing the truncation (th, tv).
                
            In general h = K((v+tv)^1/3) - th, where K = H/(V^1/3).
        """
        self.__m_Tip = tip
        ConicalProfile.__init__ (self, fullHeight + tip.FullHeight(), fullVolume + tip.FullVolume(), baseIsSmaller)

    # -----------------------------------------------------------------------------
    def _HeightOf (self, volume):
        """Determine the height of the meniscus."""
        return ConicalProfile._HeightOf (self, volume + self.__m_Tip.FullVolume()) - self.__m_Tip.FullHeight()

    # -----------------------------------------------------------------------------
    def FullHeight (self):
        """Obtain the full height of the profile"""
        return ConicalProfile.FullHeight (self) - self.__m_Tip.FullHeight()

    # -----------------------------------------------------------------------------
    def FullVolume (self):
        """Obtain the full volume of the profile"""
        return ConicalProfile.FullVolume (self) - self.__m_Tip.FullVolume()

# -----------------------------------------------------------------------------

class HemisphericalProfile (VolumeProfile):
    """Providing the height of a volume of fluid in a hemispherical section."""
    
    # -----------------------------------------------------------------------------
    def __init__(self, fullVolume, tolerance = 0.01):
        """ Define a cone (for now):
                fullVolume - the volume corresponding to maxHeight (V = v(H) = 2*pi*H^3/3).
                tolerance - the required accuracy of the height calculation. (The default is 0.01 = 1%)
                
            For a hemisphere, v = pi*(R*h^2 - h^3/3), where R = H.
                OK for calculating v, but not pretty for h!
        """
        fullHeight = (fullVolume*3.0/2.0/math.pi)**(1.0/3.0)
        VolumeProfile.__init__ (self, fullHeight, fullVolume)
        self.__coneApprox = ConicalProfile (fullHeight, fullVolume)
        self.__tolerance = tolerance

    # -----------------------------------------------------------------------------
    def _HeightOf (self, volume):
        """Determine the height of the meniscus by approximation."""
        r = self.FullHeight()

        # Assume a conical profile for a first guess at the height
        #
        approxHeight = self.__coneApprox.HeightOf(volume)
        checkVol = self._VolumeOf (approxHeight)

        while abs(checkVol - volume) > self.__tolerance * volume:
            # Refine approximation to within tolerance
            #
            approxHeight = approxHeight - (checkVol-volume)/self._VolumeRate(approxHeight)
            checkVol = self._VolumeOf (approxHeight)
            
        return approxHeight

    # -----------------------------------------------------------------------------
    def _VolumeOf (self, height):
        """Determine the volume of the fluid of a given height."""
        r = self.FullHeight()
        return math.pi*(height**2)*(r - height/3.0)
    
    # -----------------------------------------------------------------------------
    def _VolumeRate (self, height):
        """Determine the rate of volume change at a given height."""
        r = self.FullHeight()
        return math.pi*height*(2*r - height)
    
# eof

