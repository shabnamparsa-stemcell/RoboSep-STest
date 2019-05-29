# File: t (Python 2.3)

__doc__ = " test_path.py - Test the path module.\n\nThis only runs on Posix and NT right now.  I would like to have more\ntests.  You can help!  Just add appropriate pathnames for your\nplatform (os.name) in each place where the p() function is called.\nThen send me the result.  If you can't get the test to run at all on\nyour platform, there's probably a bug in path.py -- please let me\nknow!\n\nTempDirTestCase.testTouch() takes a while to run.  It sleeps a few\nseconds to allow some time to pass between calls to check the modify\ntime on files.\n\nURL:     http://www.jorendorff.com/articles/python/path\nAuthor:  Jason Orendorff <jason@jorendorff.com>\nDate:    7 Mar 2004\n\n"
import unittest
import codecs
import os
import random
import shutil
import tempfile
import time
from path import path, __version__ as path_version
__version__ = '2.0.3'

def p(**choices):
    ''' Choose a value from several possible values, based on os.name '''
    return choices[os.name]


class BasicTestCase(unittest.TestCase):
    
    def testRelpath(self):
        root = path(p(nt = 'C:\\', posix = '/'))
        foo = root / 'foo'
        quux = foo / 'quux'
        bar = foo / 'bar'
        boz = bar / 'Baz' / 'Boz'
        up = path(os.pardir)
        self.assertEqual(root.relpathto(boz), path('foo') / 'bar' / 'Baz' / 'Boz')
        self.assertEqual(bar.relpathto(boz), path('Baz') / 'Boz')
        self.assertEqual(quux.relpathto(boz), up / 'bar' / 'Baz' / 'Boz')
        self.assertEqual(boz.relpathto(quux), up / up / up / 'quux')
        self.assertEqual(boz.relpathto(bar), up / up)
        self.assertEqual(root.relpathto(root), os.curdir)
        self.assertEqual(boz.relpathto(boz), os.curdir)
        self.assertEqual(boz.relpathto(boz.normcase()), os.curdir)
        cwd = path(os.getcwd())
        self.assertEqual(boz.relpath(), cwd.relpathto(boz))
        if os.name == 'nt':
            d = path('D:\\')
            self.assertEqual(d.relpathto(boz), boz)
        

    
    def testStringCompatibility(self):
        ''' Test compatibility with ordinary strings. '''
        x = path('xyzzy')
        self.assert_(x == 'xyzzy')
        self.assert_(x == u'xyzzy')
        items = [
            path('fhj'),
            path('fgh'),
            'E',
            path('d'),
            'A',
            path('B'),
            'c']
        items.sort()
        self.assert_(items == [
            'A',
            'B',
            'E',
            'c',
            'd',
            'fgh',
            'fhj'])

    
    def testProperties(self):
        f = p(nt = 'C:\\Program Files\\Python\\Lib\\xyzzy.py', posix = '/usr/local/python/lib/xyzzy.py')
        f = path(f)
        self.assertEqual(f.parent, p(nt = 'C:\\Program Files\\Python\\Lib', posix = '/usr/local/python/lib'))
        self.assertEqual(f.name, 'xyzzy.py')
        self.assertEqual(f.parent.name, p(nt = 'Lib', posix = 'lib'))
        self.assertEqual(f.ext, '.py')
        self.assertEqual(f.parent.ext, '')
        self.assertEqual(f.drive, p(nt = 'C:', posix = ''))

    
    def testMethods(self):
        self.assertEqual(path(os.curdir).abspath(), os.getcwd())
        cwd = path.getcwd()
        self.assert_(isinstance(cwd, path))
        self.assertEqual(cwd, os.getcwd())

    
    def testUNC(self):
        if hasattr(os.path, 'splitunc'):
            p = path('\\\\python1\\share1\\dir1\\file1.txt')
            self.assert_(p.uncshare == '\\\\python1\\share1')
            self.assert_(p.splitunc() == os.path.splitunc(str(p)))
        



class TempDirTestCase(unittest.TestCase):
    
    def setUp(self):
        f = tempfile.mktemp()
        system_tmp_dir = os.path.dirname(f)
        my_dir = 'testpath_tempdir_' + str(random.random())[2:]
        self.tempdir = os.path.join(system_tmp_dir, my_dir)
        os.mkdir(self.tempdir)

    
    def tearDown(self):
        shutil.rmtree(self.tempdir)

    
    def testTouch(self):
        d = path(self.tempdir)
        f = d / 'test.txt'
        t0 = time.time() - 3
        f.touch()
        t1 = time.time() + 3
        
        try:
            self.assert_(f.exists())
            self.assert_(f.isfile())
            self.assertEqual(f.size, 0)
            if t0 <= f.mtime:
                pass
            f.mtime <= t1
            self.assert_(1)
            if hasattr(os.path, 'getctime'):
                ct = f.ctime
                if t0 <= ct:
                    pass
                ct <= t1
                self.assert_(1)
            
            time.sleep(5)
            fobj = file(f, 'ab')
            fobj.write('some bytes')
            fobj.close()
            time.sleep(5)
            t2 = time.time() - 3
            f.touch()
            t3 = time.time() + 3
            if t0 <= t1 and t1 < t2:
                pass
            t2 <= t3
            if not 1:
                raise AssertionError
            self.assert_(f.exists())
            self.assert_(f.isfile())
            self.assertEqual(f.size, 10)
            if t2 <= f.mtime:
                pass
            f.mtime <= t3
            self.assert_(1)
            if hasattr(os.path, 'getctime'):
                ct2 = f.ctime
                if os.name == 'nt':
                    self.assertEqual(ct, ct2)
                    self.assert_(ct2 < t2)
                elif not ct == ct2:
                    pass
                self.failUnless(ct2 == f.mtime)
        finally:
            f.remove()


    
    def testListing(self):
        d = path(self.tempdir)
        self.assertEqual(d.listdir(), [])
        f = 'testfile.txt'
        af = d / f
        self.assertEqual(af, os.path.join(d, f))
        af.touch()
        
        try:
            self.assert_(af.exists())
            self.assertEqual(d.listdir(), [
                af])
            self.assertEqual(d.glob('testfile.txt'), [
                af])
            self.assertEqual(d.glob('test*.txt'), [
                af])
            self.assertEqual(d.glob('*.txt'), [
                af])
            self.assertEqual(d.glob('*txt'), [
                af])
            self.assertEqual(d.glob('*'), [
                af])
            self.assertEqual(d.glob('*.html'), [])
            self.assertEqual(d.glob('testfile'), [])
        finally:
            af.remove()

        continue
        files = [ d / ('%d.txt' % i) for i in range(20) ]
        for f in files:
            fobj = file(f, 'w')
            fobj.write('some text\n')
            fobj.close()
        
        
        try:
            files2 = d.listdir()
            files.sort()
            files2.sort()
            self.assertEqual(files, files2)
        finally:
            for f in files:
                
                try:
                    f.remove()
                continue
                []
                continue

            


    
    def testMakeDirs(self):
        d = path(self.tempdir)
        tempf = d / 'temp.txt'
        tempf.touch()
        
        try:
            foo = d / 'foo'
            boz = foo / 'bar' / 'baz' / 'boz'
            boz.makedirs()
            
            try:
                self.assert_(boz.isdir())
            finally:
                boz.removedirs()

            self.failIf(foo.exists())
            self.assert_(d.exists())
            foo.mkdir(488)
            boz.makedirs(448)
            
            try:
                self.assert_(boz.isdir())
            finally:
                boz.removedirs()

            self.failIf(foo.exists())
            self.assert_(d.exists())
        finally:
            os.remove(tempf)


    
    def assertSetsEqual(self, a, b):
        ad = { }
        for i in a:
            ad[i] = None
        
        bd = { }
        for i in b:
            bd[i] = None
        
        self.assertEqual(ad, bd)

    
    def testShutil(self):
        d = path(self.tempdir)
        testDir = d / 'testdir'
        testFile = testDir / 'testfile.txt'
        testA = testDir / 'A'
        testCopy = testA / 'testcopy.txt'
        testLink = testA / 'testlink.txt'
        testB = testDir / 'B'
        testC = testB / 'C'
        testCopyOfLink = testC / testA.relpathto(testLink)
        testDir.mkdir()
        testA.mkdir()
        testB.mkdir()
        f = open(testFile, 'w')
        f.write('x' * 10000)
        f.close()
        testFile.copyfile(testCopy)
        self.assert_(testCopy.isfile())
        self.assert_(testFile.bytes() == testCopy.bytes())
        testCopy2 = testA / testFile.name
        testFile.copy(testA)
        self.assert_(testCopy2.isfile())
        self.assert_(testFile.bytes() == testCopy2.bytes())
        if hasattr(os, 'symlink'):
            testFile.symlink(testLink)
        else:
            testFile.copy(testLink)
        testA.copytree(testC)
        self.assert_(testC.isdir())
        self.assertSetsEqual(testC.listdir(), [
            testC / testCopy.name,
            testC / testFile.name,
            testCopyOfLink])
        self.assert_(not testCopyOfLink.islink())
        testC.rmtree()
        self.assert_(not testC.exists())
        testA.copytree(testC, True)
        self.assert_(testC.isdir())
        self.assertSetsEqual(testC.listdir(), [
            testC / testCopy.name,
            testC / testFile.name,
            testCopyOfLink])
        if hasattr(os, 'symlink'):
            self.assert_(testCopyOfLink.islink())
            self.assert_(testCopyOfLink.readlink() == testFile)
        
        testDir.rmtree()
        self.assert_(not testDir.exists())
        self.assertList(d.listdir(), [])

    
    def assertList(self, listing, expected):
        listing = list(listing)
        listing.sort()
        expected = list(expected)
        expected.sort()
        self.assertEqual(listing, expected)

    
    def testPatterns(self):
        d = path(self.tempdir)
        names = [
            'x.tmp',
            'x.xtmp',
            'x2g',
            'x22',
            'x.txt']
        dirs = [
            d,
            d / 'xdir',
            d / 'xdir.tmp',
            d / 'xdir.tmp' / 'xsubdir']
        for e in dirs:
            if not e.isdir():
                e.makedirs()
            
            for name in names:
                (e / name).touch()
            
        
        self.assertList(d.listdir('*.tmp'), [
            d / 'x.tmp',
            d / 'xdir.tmp'])
        self.assertList(d.files('*.tmp'), [
            d / 'x.tmp'])
        self.assertList(d.dirs('*.tmp'), [
            d / 'xdir.tmp'])
        continue
        continue
        self.assertList(d.walk(), [] + [ e / n for e in dirs for n in names ])
        continue
        d.walk('*.tmp')([], [ e / 'x.tmp' for e in dirs ] + [
            d / 'xdir.tmp'])
        continue
        d.walkfiles('*.tmp')([], [ e / 'x.tmp' for e in dirs ])
        self.assertList(d.walkdirs('*.tmp'), [
            d / 'xdir.tmp'])

    
    def testUnicode(self):
        d = path(self.tempdir)
        p = d / 'unicode.txt'
        
        def test(enc):
            ''' Test that path works with the specified encoding,
            which must be capable of representing the entire range of
            Unicode codepoints.
            '''
            given = u'Hello world\n\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\r\n\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\xc2\x85\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\xe2\x80\xa8\rhanging'
            clean = u'Hello world\n\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\n\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\n\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\n\nhanging'
            givenLines = [
                u'Hello world\n',
                u'\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\r\n',
                u'\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\xc2\x85',
                u'\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\xe2\x80\xa8',
                u'\r',
                u'hanging']
            expectedLines = [
                u'Hello world\n',
                u'\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\n',
                u'\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\n',
                u'\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95\n',
                u'\n',
                u'hanging']
            expectedLines2 = [
                u'Hello world',
                u'\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95',
                u'\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95',
                u'\xe0\xb4\x8a\xe0\xa8\x8d\xe0\xb4\x95\xe0\xa8\x95',
                u'',
                u'hanging']
            f = codecs.open(p, 'w', enc)
            f.write(given)
            f.close()
            self.assertEqual(p.bytes(), given.encode(enc))
            self.assertEqual(p.text(enc), clean)
            self.assertEqual(p.lines(enc), expectedLines)
            self.assertEqual(p.lines(enc, retain = False), expectedLines2)
            if enc == 'UTF-16':
                return None
            
            cleanNoHanging = clean + u'\n'
            p.write_text(cleanNoHanging, enc)
            p.write_text(cleanNoHanging, enc, append = True)
            expectedBytes = 2 * cleanNoHanging.replace('\n', os.linesep).encode(enc)
            expectedLinesNoHanging = expectedLines[:]
            expectedLinesNoHanging[-1] += '\n'
            self.assertEqual(p.bytes(), expectedBytes)
            self.assertEqual(p.text(enc), 2 * cleanNoHanging)
            self.assertEqual(p.lines(enc), 2 * expectedLinesNoHanging)
            self.assertEqual(p.lines(enc, retain = False), 2 * expectedLines2)
            p.write_lines(expectedLines, enc)
            p.write_lines(expectedLines2, enc, append = True)
            self.assertEqual(p.bytes(), expectedBytes)
            p.write_lines(givenLines, enc)
            p.write_lines(givenLines, enc, append = True)
            self.assertEqual(p.bytes(), expectedBytes)
            p.write_lines(givenLines, enc, linesep = None)
            p.write_lines(givenLines, enc, linesep = None, append = True)
            expectedBytes = 2 * given.encode(enc)
            self.assertEqual(p.bytes(), expectedBytes)
            self.assertEqual(p.text(enc), 2 * clean)
            expectedResultLines = expectedLines[:]
            expectedResultLines[-1] += expectedLines[0]
            expectedResultLines += expectedLines[1:]
            self.assertEqual(p.lines(enc), expectedResultLines)

        test('UTF-8')
        test('UTF-16BE')
        test('UTF-16LE')
        test('UTF-16')


if __name__ == '__main__':
    if __version__ != path_version:
        print 'Version mismatch:  test_path.py version %s, path version %s' % (__version__, path_version)
    
    unittest.main()

