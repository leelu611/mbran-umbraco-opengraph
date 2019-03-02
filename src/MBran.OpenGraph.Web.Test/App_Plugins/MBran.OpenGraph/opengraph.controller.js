angular.module('umbraco')
    .controller('MBran.OpenGraph.OpenGraphController', function ($scope, $element, assetsService, dialogService, entityResource) {
        

        function init() {
            $scope.model.hideLabel = true;
            initOpenGraphModels();

            if (!$scope.model.value) {
                $scope.model.value = {};
            }

            if (!$scope.model.value.metadata) {
                $scope.model.value.metadata = [];
            }

            if (!$scope.model.value.image) {
                initThumbnailModel();
            }
            else {
                entityResource.getById($scope.model.value.image, "Media").then(function (ent) {
                    $scope.thumbnail = {};
                    $scope.thumbnail.src = ent.metaData.umbracoFile.Value.src;
                    $scope.thumbnail.width = ent.metaData.umbracoWidth.Value;
                    $scope.thumbnail.height = ent.metaData.umbracoHeight.Value;
                });
            }

        }

        function initOpenGraphModels() {
            $scope.opengraph = {};
            $scope.opengraph.types = {
                "music.song": "Music: Song",
                "music.album": "Music: Album",
                "music.playlist": "Music: Playlist",
                "music.radio_station": "Music: Radio Station",
                "video.movie": "Video: Movie",
                "video.episode": "Video: Episode",
                "video.tv_show": "Video: Tv Show",
                "video.other": "Video: Other",
                "article": "Article",
                "book": "Book",
                "profile": "Profile",
                "website": "Website",
                "blog": "Blog",
                "game": "Game",
                "movie": "Movie",
                "food": "Food",
                "city": "City",
                "country": "Country",
                "company": "Company",
                "hotel": "Hotel",
                "restaurant": "Restaurant",
            };
            
            $scope.metadata = {
                key: '',
                value: ''
            };
        }
        function initThumbnailModel() {
            $scope.thumbnail = {};
            $scope.thumbnail.src = '';
            $scope.thumbnail.width = 0;
            $scope.thumbnail.height = 0;
        }

        $scope.pickImage = function () {
            dialogService.mediaPicker({
                multiPicker: false,
                callback: function (data) {
                    $scope.model.value.image = data.id;
                    $scope.thumbnail.src = data.thumbnail;
                    $scope.thumbnail.width = data.originalWidth;
                    $scope.thumbnail.height = data.originalHeight;

                }
            });
        };
        $scope.removeImage = function () {
            $scope.thumbnail.src = '';
            $scope.model.value.image = '';
        };

        $scope.removeMetadata = function(index) {
            delete $scope.model.value.metadata[index];
            $scope.model.value.metadata = $scope.model.value.metadata.filter(Boolean);

        }

        $scope.addMetadata = function() {
            $scope.model.value.metadata.push({
                "key": $scope.metadata.key,
                "value": $scope.metadata.value
            });
            $scope.metadata.key = '';
            $scope.metadata.value = '';
        }
        init();
    });