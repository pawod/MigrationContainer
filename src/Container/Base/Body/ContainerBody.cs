namespace DataMigrator.Container.Base.Body
{
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Header;
    using Helper;
    using Stream;

    /// <summary>
    ///     Provides access to a container's content.
    /// </summary>
    public class ContainerBody : IContainerBody

    {
        private readonly List<FileInfo> _allParts;
        private readonly long _mainPartBodyPosition;
        private readonly long _mainPartLength;
	    private readonly int _contentBufferSize;

	    /// <summary>
	    ///     Initializes a new ContainerBody instance.
	    /// </summary>
	    /// <param name="mainPartBodyPosition">
	    ///     The stream position index, where the body of
	    ///     the main part starts.
	    /// </param>
	    /// <param name="allParts">A complete & ordered list of all container files.</param>
	    /// <param name="contentBufferSize">The buffer size to be used, when accessing the body</param>
	    public ContainerBody(long mainPartBodyPosition, List<FileInfo> allParts, int contentBufferSize)
        {
            _mainPartLength = allParts[0].Length;
            _mainPartBodyPosition = mainPartBodyPosition;
            _allParts = allParts;
		    _contentBufferSize = contentBufferSize;
        }

        public void Extract(IContentHeader contentHeader, Stream targetStream)
        {
            using (var compositeStream = GetCompositeStream(contentHeader)) compositeStream.CopyTo(targetStream, _contentBufferSize);
        }

        public void Extract(IContentHeader contentHeader, Stream targetStream, long startOffset, long count)
        {
            using (var compositeStream = GetCompositeStream(contentHeader))
            {
                compositeStream.CopyTo(targetStream,
                    startOffset,
                    targetStream.Position,
                    count,
                    _contentBufferSize);
            }
        }

        public CompositeStream GetCompositeStream(IContentHeader contentHeader)
        {
            var subStreams = GetSubStreams(contentHeader);
            return new CompositeStream(subStreams);
        }

        private int GetContainingPart(long cumulatedContentPosition)
        {
            return (int)Math.Floor(cumulatedContentPosition/(double)_mainPartLength);
        }

        private long GetCummulatedContentPosition(long contentOffset, out int part)
        {
            /* Desired content is located in the main part. Either it's starts right at the beginning 
             * of the content (offset == 0) or later (offset > 0 && offset < mainPartLength) */
            if (_allParts.Count == 1 || _mainPartBodyPosition + contentOffset < _mainPartLength)
            {
                part = 0;
                return _mainPartBodyPosition + contentOffset;
            }

            /* In order to determine the absolute position of the content it is required to calculate
			 * the occuring overhead, caused by the container's headers. The overhead can easily be
			 * calculated by determining the number of files, that would have been required to save
			 * the bytes between the main part and the content's offset.*/

            var contentPosition = contentOffset + _mainPartBodyPosition;
            var distanceToMainPart = Math.Abs(_mainPartLength - contentPosition);

            // desired content is located directly next to the main part
            if (distanceToMainPart == 0)
            {
                part = 1;
                return _mainPartLength + StartHeader.Length;
            }

            var numberOfFiles = (long)Math.Ceiling((distanceToMainPart/(double)_mainPartLength));
            var overhead = numberOfFiles*StartHeader.Length;

            var cumulatedContentPosition = (contentPosition + overhead);

            // required, when cumulated position exceeds boundary of current part
            while ((part = GetContainingPart(cumulatedContentPosition)) > numberOfFiles)
            {
                var additionalParts = part - numberOfFiles;
                cumulatedContentPosition += StartHeader.Length*additionalParts;
                numberOfFiles = part;
            }
            return cumulatedContentPosition;
        }

        private List<SubStream> GetSubStreams(IContentHeader contentHeader)
        {
            var count = 0L;
            var offset = contentHeader.ContentOffset;
            var subStreams = new List<SubStream>();

            while (count < contentHeader.ContentLength)
            {
                int part;
                var cumulatedContentPosition = GetCummulatedContentPosition(offset, out part);

                var cumulatedPartPosition = _mainPartLength*(part);
                var relativePosition = cumulatedContentPosition - cumulatedPartPosition;
                var contentStream = new FileInfo(_allParts[part].FullName).OpenRead();

                var remainingStreamLength = contentStream.Length - relativePosition;
                var remainingContentLength = contentHeader.ContentLength - count;
                var subStreamLength = (remainingStreamLength >= remainingContentLength)
                    ? remainingContentLength
                    : remainingStreamLength;

                subStreams.Add(contentStream.GetSubStream(relativePosition, subStreamLength));
                offset += subStreamLength;
                count += subStreamLength;
            }
            return subStreams;
        }
    }
}
