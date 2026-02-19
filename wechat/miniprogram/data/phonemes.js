// data/phonemes.js
// 音律方舟 - 完整 IPA 音标数据（44 个音标）

const vowels = [
  {
    symbol: 'iː',
    type: 'vowel',
    name: '长元音',
    description: '长元音，舌位高前，如 see',
    examples: [
      { word: 'see', ipa: '/siː/' },
      { word: 'sea', ipa: '/siː/' },
      { word: 'meet', ipa: '/miːt/' },
      { word: 'key', ipa: '/kiː/' },
      { word: 'tree', ipa: '/triː/' }
    ]
  },
  {
    symbol: 'ɪ',
    type: 'vowel',
    name: '短元音',
    description: '短元音，舌位高前偏低，如 sit',
    examples: [
      { word: 'sit', ipa: '/sɪt/' },
      { word: 'hit', ipa: '/hɪt/' },
      { word: 'big', ipa: '/bɪɡ/' },
      { word: 'ship', ipa: '/ʃɪp/' },
      { word: 'fish', ipa: '/fɪʃ/' }
    ]
  },
  {
    symbol: 'e',
    type: 'vowel',
    name: '短元音',
    description: '短元音，舌位中前，如 bed',
    examples: [
      { word: 'bed', ipa: '/bed/' },
      { word: 'head', ipa: '/hed/' },
      { word: 'red', ipa: '/red/' },
      { word: 'said', ipa: '/sed/' },
      { word: 'egg', ipa: '/eɡ/' }
    ]
  },
  {
    symbol: 'æ',
    type: 'vowel',
    name: '短元音',
    description: '短元音，舌位低前，如 cat',
    examples: [
      { word: 'cat', ipa: '/kæt/' },
      { word: 'hat', ipa: '/hæt/' },
      { word: 'bad', ipa: '/bæd/' },
      { word: 'man', ipa: '/mæn/' },
      { word: 'map', ipa: '/mæp/' }
    ]
  },
  {
    symbol: 'ɑː',
    type: 'vowel',
    name: '长元音',
    description: '长元音，舌位低后，如 car',
    examples: [
      { word: 'car', ipa: '/kɑːr/' },
      { word: 'far', ipa: '/fɑːr/' },
      { word: 'palm', ipa: '/pɑːm/' },
      { word: 'father', ipa: '/ˈfɑːðər/' },
      { word: 'heart', ipa: '/hɑːrt/' }
    ]
  },
  {
    symbol: 'ɒ',
    type: 'vowel',
    name: '短元音',
    description: '短元音，舌位低后圆唇，如 hot',
    examples: [
      { word: 'hot', ipa: '/hɒt/' },
      { word: 'lot', ipa: '/lɒt/' },
      { word: 'dog', ipa: '/dɒɡ/' },
      { word: 'box', ipa: '/bɒks/' },
      { word: 'stop', ipa: '/stɒp/' }
    ]
  },
  {
    symbol: 'ɔː',
    type: 'vowel',
    name: '长元音',
    description: '长元音，舌位中后圆唇，如 saw',
    examples: [
      { word: 'saw', ipa: '/sɔː/' },
      { word: 'law', ipa: '/lɔː/' },
      { word: 'four', ipa: '/fɔːr/' },
      { word: 'door', ipa: '/dɔːr/' },
      { word: 'wall', ipa: '/wɔːl/' }
    ]
  },
  {
    symbol: 'ʊ',
    type: 'vowel',
    name: '短元音',
    description: '短元音，舌位高后偏低，如 book',
    examples: [
      { word: 'book', ipa: '/bʊk/' },
      { word: 'look', ipa: '/lʊk/' },
      { word: 'put', ipa: '/pʊt/' },
      { word: 'good', ipa: '/ɡʊd/' },
      { word: 'foot', ipa: '/fʊt/' }
    ]
  },
  {
    symbol: 'uː',
    type: 'vowel',
    name: '长元音',
    description: '长元音，舌位高后圆唇，如 food',
    examples: [
      { word: 'food', ipa: '/fuːd/' },
      { word: 'moon', ipa: '/muːn/' },
      { word: 'blue', ipa: '/bluː/' },
      { word: 'true', ipa: '/truː/' },
      { word: 'cool', ipa: '/kuːl/' }
    ]
  },
  {
    symbol: 'ʌ',
    type: 'vowel',
    name: '短元音',
    description: '短元音，舌位中偏低，如 cup',
    examples: [
      { word: 'cup', ipa: '/kʌp/' },
      { word: 'love', ipa: '/lʌv/' },
      { word: 'sun', ipa: '/sʌn/' },
      { word: 'but', ipa: '/bʌt/' },
      { word: 'run', ipa: '/rʌn/' }
    ]
  },
  {
    symbol: 'ɜː',
    type: 'vowel',
    name: '长元音',
    description: '长元音，舌位中央，如 bird',
    examples: [
      { word: 'bird', ipa: '/bɜːrd/' },
      { word: 'her', ipa: '/hɜːr/' },
      { word: 'learn', ipa: '/lɜːrn/' },
      { word: 'turn', ipa: '/tɜːrn/' },
      { word: 'work', ipa: '/wɜːrk/' }
    ]
  },
  {
    symbol: 'ə',
    type: 'vowel',
    name: '中央元音',
    description: '中央元音（schwa），非重读音节常见，如 about',
    examples: [
      { word: 'about', ipa: '/əˈbaʊt/' },
      { word: 'sofa', ipa: '/ˈsoʊfə/' },
      { word: 'china', ipa: '/ˈtʃaɪnə/' },
      { word: 'above', ipa: '/əˈbʌv/' },
      { word: 'banana', ipa: '/bəˈnænə/' }
    ]
  }
];

const diphthongs = [
  {
    symbol: 'eɪ',
    type: 'diphthong',
    name: '双元音',
    description: '双元音，从 e 滑向 ɪ，如 day',
    examples: [
      { word: 'day', ipa: '/deɪ/' },
      { word: 'make', ipa: '/meɪk/' },
      { word: 'say', ipa: '/seɪ/' },
      { word: 'rain', ipa: '/reɪn/' },
      { word: 'play', ipa: '/pleɪ/' }
    ]
  },
  {
    symbol: 'aɪ',
    type: 'diphthong',
    name: '双元音',
    description: '双元音，从 a 滑向 ɪ，如 my',
    examples: [
      { word: 'my', ipa: '/maɪ/' },
      { word: 'time', ipa: '/taɪm/' },
      { word: 'high', ipa: '/haɪ/' },
      { word: 'buy', ipa: '/baɪ/' },
      { word: 'fly', ipa: '/flaɪ/' }
    ]
  },
  {
    symbol: 'ɔɪ',
    type: 'diphthong',
    name: '双元音',
    description: '双元音，从 ɔ 滑向 ɪ，如 boy',
    examples: [
      { word: 'boy', ipa: '/bɔɪ/' },
      { word: 'toy', ipa: '/tɔɪ/' },
      { word: 'coin', ipa: '/kɔɪn/' },
      { word: 'voice', ipa: '/vɔɪs/' },
      { word: 'enjoy', ipa: '/ɪnˈdʒɔɪ/' }
    ]
  },
  {
    symbol: 'aʊ',
    type: 'diphthong',
    name: '双元音',
    description: '双元音，从 a 滑向 ʊ，如 now',
    examples: [
      { word: 'now', ipa: '/naʊ/' },
      { word: 'how', ipa: '/haʊ/' },
      { word: 'house', ipa: '/haʊs/' },
      { word: 'town', ipa: '/taʊn/' },
      { word: 'cloud', ipa: '/klaʊd/' }
    ]
  },
  {
    symbol: 'oʊ',
    type: 'diphthong',
    name: '双元音',
    description: '双元音，从 o 滑向 ʊ，如 go',
    examples: [
      { word: 'go', ipa: '/ɡoʊ/' },
      { word: 'home', ipa: '/hoʊm/' },
      { word: 'know', ipa: '/noʊ/' },
      { word: 'show', ipa: '/ʃoʊ/' },
      { word: 'road', ipa: '/roʊd/' }
    ]
  },
  {
    symbol: 'ɪə',
    type: 'diphthong',
    name: '双元音',
    description: '双元音，从 ɪ 滑向 ə，如 here',
    examples: [
      { word: 'here', ipa: '/hɪər/' },
      { word: 'near', ipa: '/nɪər/' },
      { word: 'ear', ipa: '/ɪər/' },
      { word: 'deer', ipa: '/dɪər/' },
      { word: 'clear', ipa: '/klɪər/' }
    ]
  },
  {
    symbol: 'eə',
    type: 'diphthong',
    name: '双元音',
    description: '双元音，从 e 滑向 ə，如 hair',
    examples: [
      { word: 'hair', ipa: '/heər/' },
      { word: 'care', ipa: '/keər/' },
      { word: 'bear', ipa: '/beər/' },
      { word: 'where', ipa: '/weər/' },
      { word: 'share', ipa: '/ʃeər/' }
    ]
  },
  {
    symbol: 'ʊə',
    type: 'diphthong',
    name: '双元音',
    description: '双元音，从 ʊ 滑向 ə，如 poor',
    examples: [
      { word: 'poor', ipa: '/pʊər/' },
      { word: 'sure', ipa: '/ʃʊər/' },
      { word: 'tour', ipa: '/tʊər/' },
      { word: 'cure', ipa: '/kjʊər/' },
      { word: 'pure', ipa: '/pjʊər/' }
    ]
  }
];

const consonants = [
  {
    symbol: 'p',
    type: 'consonant',
    name: '清辅音',
    description: '清双唇塞音，如 pen',
    examples: [
      { word: 'pen', ipa: '/pen/' },
      { word: 'apple', ipa: '/ˈæpəl/' },
      { word: 'top', ipa: '/tɒp/' },
      { word: 'happy', ipa: '/ˈhæpi/' },
      { word: 'cup', ipa: '/kʌp/' }
    ]
  },
  {
    symbol: 'b',
    type: 'consonant',
    name: '浊辅音',
    description: '浊双唇塞音，如 bad',
    examples: [
      { word: 'bad', ipa: '/bæd/' },
      { word: 'baby', ipa: '/ˈbeɪbi/' },
      { word: 'job', ipa: '/dʒɒb/' },
      { word: 'rabbit', ipa: '/ˈræbɪt/' },
      { word: 'cab', ipa: '/kæb/' }
    ]
  },
  {
    symbol: 't',
    type: 'consonant',
    name: '清辅音',
    description: '清齿龈塞音，如 top',
    examples: [
      { word: 'top', ipa: '/tɒp/' },
      { word: 'time', ipa: '/taɪm/' },
      { word: 'cat', ipa: '/kæt/' },
      { word: 'better', ipa: '/ˈbetər/' },
      { word: 'hat', ipa: '/hæt/' }
    ]
  },
  {
    symbol: 'd',
    type: 'consonant',
    name: '浊辅音',
    description: '浊齿龈塞音，如 dog',
    examples: [
      { word: 'dog', ipa: '/dɒɡ/' },
      { word: 'day', ipa: '/deɪ/' },
      { word: 'red', ipa: '/red/' },
      { word: 'middle', ipa: '/ˈmɪdəl/' },
      { word: 'bad', ipa: '/bæd/' }
    ]
  },
  {
    symbol: 'k',
    type: 'consonant',
    name: '清辅音',
    description: '清软腭塞音，如 cat',
    examples: [
      { word: 'cat', ipa: '/kæt/' },
      { word: 'key', ipa: '/kiː/' },
      { word: 'back', ipa: '/bæk/' },
      { word: 'school', ipa: '/skuːl/' },
      { word: 'make', ipa: '/meɪk/' }
    ]
  },
  {
    symbol: 'ɡ',
    type: 'consonant',
    name: '浊辅音',
    description: '浊软腭塞音，如 go',
    examples: [
      { word: 'go', ipa: '/ɡoʊ/' },
      { word: 'good', ipa: '/ɡʊd/' },
      { word: 'big', ipa: '/bɪɡ/' },
      { word: 'dog', ipa: '/dɒɡ/' },
      { word: 'egg', ipa: '/eɡ/' }
    ]
  },
  {
    symbol: 'f',
    type: 'consonant',
    name: '清辅音',
    description: '清唇齿擦音，如 fish',
    examples: [
      { word: 'fish', ipa: '/fɪʃ/' },
      { word: 'fun', ipa: '/fʌn/' },
      { word: 'off', ipa: '/ɒf/' },
      { word: 'photo', ipa: '/ˈfoʊtoʊ/' },
      { word: 'leaf', ipa: '/liːf/' }
    ]
  },
  {
    symbol: 'v',
    type: 'consonant',
    name: '浊辅音',
    description: '浊唇齿擦音，如 van',
    examples: [
      { word: 'van', ipa: '/væn/' },
      { word: 'very', ipa: '/ˈveri/' },
      { word: 'love', ipa: '/lʌv/' },
      { word: 'have', ipa: '/hæv/' },
      { word: 'five', ipa: '/faɪv/' }
    ]
  },
  {
    symbol: 'θ',
    type: 'consonant',
    name: '清辅音',
    description: '清齿擦音，如 think',
    examples: [
      { word: 'think', ipa: '/θɪŋk/' },
      { word: 'thank', ipa: '/θæŋk/' },
      { word: 'bath', ipa: '/bæθ/' },
      { word: 'truth', ipa: '/truːθ/' },
      { word: 'mouth', ipa: '/maʊθ/' }
    ]
  },
  {
    symbol: 'ð',
    type: 'consonant',
    name: '浊辅音',
    description: '浊齿擦音，如 this',
    examples: [
      { word: 'this', ipa: '/ðɪs/' },
      { word: 'that', ipa: '/ðæt/' },
      { word: 'mother', ipa: '/ˈmʌðər/' },
      { word: 'with', ipa: '/wɪð/' },
      { word: 'other', ipa: '/ˈʌðər/' }
    ]
  },
  {
    symbol: 's',
    type: 'consonant',
    name: '清辅音',
    description: '清齿龈擦音，如 sit',
    examples: [
      { word: 'sit', ipa: '/sɪt/' },
      { word: 'see', ipa: '/siː/' },
      { word: 'house', ipa: '/haʊs/' },
      { word: 'pass', ipa: '/pæs/' },
      { word: 'miss', ipa: '/mɪs/' }
    ]
  },
  {
    symbol: 'z',
    type: 'consonant',
    name: '浊辅音',
    description: '浊齿龈擦音，如 zoo',
    examples: [
      { word: 'zoo', ipa: '/zuː/' },
      { word: 'zero', ipa: '/ˈzɪəroʊ/' },
      { word: 'has', ipa: '/hæz/' },
      { word: 'easy', ipa: '/ˈiːzi/' },
      { word: 'buzz', ipa: '/bʌz/' }
    ]
  },
  {
    symbol: 'ʃ',
    type: 'consonant',
    name: '清辅音',
    description: '清龈后擦音，如 ship',
    examples: [
      { word: 'ship', ipa: '/ʃɪp/' },
      { word: 'shoe', ipa: '/ʃuː/' },
      { word: 'wash', ipa: '/wɒʃ/' },
      { word: 'fish', ipa: '/fɪʃ/' },
      { word: 'push', ipa: '/pʊʃ/' }
    ]
  },
  {
    symbol: 'ʒ',
    type: 'consonant',
    name: '浊辅音',
    description: '浊龈后擦音，如 measure',
    examples: [
      { word: 'measure', ipa: '/ˈmeʒər/' },
      { word: 'vision', ipa: '/ˈvɪʒən/' },
      { word: 'pleasure', ipa: '/ˈpleʒər/' },
      { word: 'usual', ipa: '/ˈjuːʒuəl/' },
      { word: 'garage', ipa: '/ɡəˈrɑːʒ/' }
    ]
  },
  {
    symbol: 'h',
    type: 'consonant',
    name: '清辅音',
    description: '清声门擦音，如 hat',
    examples: [
      { word: 'hat', ipa: '/hæt/' },
      { word: 'home', ipa: '/hoʊm/' },
      { word: 'who', ipa: '/huː/' },
      { word: 'ahead', ipa: '/əˈhed/' },
      { word: 'behind', ipa: '/bɪˈhaɪnd/' }
    ]
  },
  {
    symbol: 'm',
    type: 'consonant',
    name: '鼻音',
    description: '双唇鼻音，如 man',
    examples: [
      { word: 'man', ipa: '/mæn/' },
      { word: 'mother', ipa: '/ˈmʌðər/' },
      { word: 'time', ipa: '/taɪm/' },
      { word: 'come', ipa: '/kʌm/' },
      { word: 'swim', ipa: '/swɪm/' }
    ]
  },
  {
    symbol: 'n',
    type: 'consonant',
    name: '鼻音',
    description: '齿龈鼻音，如 no',
    examples: [
      { word: 'no', ipa: '/noʊ/' },
      { word: 'now', ipa: '/naʊ/' },
      { word: 'sun', ipa: '/sʌn/' },
      { word: 'man', ipa: '/mæn/' },
      { word: 'ten', ipa: '/ten/' }
    ]
  },
  {
    symbol: 'ŋ',
    type: 'consonant',
    name: '鼻音',
    description: '软腭鼻音，如 sing',
    examples: [
      { word: 'sing', ipa: '/sɪŋ/' },
      { word: 'song', ipa: '/sɒŋ/' },
      { word: 'long', ipa: '/lɒŋ/' },
      { word: 'think', ipa: '/θɪŋk/' },
      { word: 'ring', ipa: '/rɪŋ/' }
    ]
  },
  {
    symbol: 'l',
    type: 'consonant',
    name: '近音',
    description: '齿龈边近音，如 leg',
    examples: [
      { word: 'leg', ipa: '/leɡ/' },
      { word: 'love', ipa: '/lʌv/' },
      { word: 'all', ipa: '/ɔːl/' },
      { word: 'feel', ipa: '/fiːl/' },
      { word: 'milk', ipa: '/mɪlk/' }
    ]
  },
  {
    symbol: 'r',
    type: 'consonant',
    name: '近音',
    description: '齿龈近音，如 red',
    examples: [
      { word: 'red', ipa: '/red/' },
      { word: 'run', ipa: '/rʌn/' },
      { word: 'very', ipa: '/ˈveri/' },
      { word: 'car', ipa: '/kɑːr/' },
      { word: 'tree', ipa: '/triː/' }
    ]
  },
  {
    symbol: 'j',
    type: 'consonant',
    name: '近音',
    description: '硬腭近音，如 yes',
    examples: [
      { word: 'yes', ipa: '/jes/' },
      { word: 'you', ipa: '/juː/' },
      { word: 'yellow', ipa: '/ˈjeloʊ/' },
      { word: 'onion', ipa: '/ˈʌnjən/' },
      { word: 'year', ipa: '/jɪər/' }
    ]
  },
  {
    symbol: 'w',
    type: 'consonant',
    name: '近音',
    description: '唇软腭近音，如 we',
    examples: [
      { word: 'we', ipa: '/wiː/' },
      { word: 'water', ipa: '/ˈwɔːtər/' },
      { word: 'away', ipa: '/əˈweɪ/' },
      { word: 'win', ipa: '/wɪn/' },
      { word: 'want', ipa: '/wɒnt/' }
    ]
  },
  {
    symbol: 'tʃ',
    type: 'consonant',
    name: '清辅音',
    description: '清龈后塞擦音，如 chair',
    examples: [
      { word: 'chair', ipa: '/tʃeər/' },
      { word: 'church', ipa: '/tʃɜːrtʃ/' },
      { word: 'watch', ipa: '/wɒtʃ/' },
      { word: 'picture', ipa: '/ˈpɪktʃər/' },
      { word: 'teach', ipa: '/tiːtʃ/' }
    ]
  },
  {
    symbol: 'dʒ',
    type: 'consonant',
    name: '浊辅音',
    description: '浊龈后塞擦音，如 judge',
    examples: [
      { word: 'judge', ipa: '/dʒʌdʒ/' },
      { word: 'job', ipa: '/dʒɒb/' },
      { word: 'page', ipa: '/peɪdʒ/' },
      { word: 'bridge', ipa: '/brɪdʒ/' },
      { word: 'age', ipa: '/eɪdʒ/' }
    ]
  }
];

/**
 * 获取所有音标数据
 * @returns {Array} 包含所有 44 个音标的数组
 */
function getAllPhonemes() {
  return [...vowels, ...diphthongs, ...consonants];
}

/**
 * 按类型获取音标
 * @param {string} type - 'vowel' | 'diphthong' | 'consonant'
 * @returns {Array} 指定类型的音标数组
 */
function getPhonemesByType(type) {
  const all = getAllPhonemes();
  return all.filter(p => p.type === type);
}

/**
 * 根据符号查找音标
 * @param {string} symbol - 音标符号，如 'iː'
 * @returns {Object|undefined} 音标对象
 */
function getPhonemeBySymbol(symbol) {
  return getAllPhonemes().find(p => p.symbol === symbol);
}

/**
 * 获取音标分类信息
 * @returns {Object} 分类统计
 */
function getPhonemeStats() {
  return {
    vowelCount: vowels.length,
    diphthongCount: diphthongs.length,
    consonantCount: consonants.length,
    totalCount: vowels.length + diphthongs.length + consonants.length
  };
}

module.exports = {
  vowels,
  diphthongs,
  consonants,
  getAllPhonemes,
  getPhonemesByType,
  getPhonemeBySymbol,
  getPhonemeStats
};
