package main

import (
	"fmt"
	"io/ioutil"
	"log"
	"strconv"
	"strings"
)

type ClassChallengeProcessor struct {
	DictionaryFile string
	SearchFile     string
	DictionaryList []string
	SearchList     []string
	CaseCount      []int
}

func (obj *ClassChallengeProcessor) Process() {
	obj.ParseDictionaryFile()
	obj.ParseSearchFile()
	obj.ProcessData()
}

func (obj *ClassChallengeProcessor) ProcessData() {
	for k := 0; k < len(obj.SearchList); k++ {
		for i := 0; i < len(obj.DictionaryList); i++ {
			var probabilityList = obj.GenerateProbabilityList(obj.DictionaryList[i])
			for j := 0; j < len(probabilityList); j++ {
				var found = false
				if strings.Contains(obj.SearchList[k], probabilityList[j]) {
					obj.CaseCount[k]++
					found = true
				}

				if found {
					break
				}
			}
		}
	}
}

func (obj *ClassChallengeProcessor) GenerateProbabilityList(input string) []string {
	var ret []string
	var start = string(input[0])
	var end = string(input[len(input)-1])

	for i := 1; i < len(input)-1; i++ {
		var token = string(input[i])
		var others = delChar(input, len(input)-1)
		others = delChar(others, i)
		others = delChar(others, 0)

		var str = start + token + string(others) + end
		if !Contains(ret, str) {
			ret = append(ret, str)
		}

		for j := 0; j < len(others)-1; j++ {
			var substr = delChar(others, len(others)-1)
			substr = string(others[len(others)-1]) + substr

			str = start + token + substr + end
			if !Contains(ret, str) {
				ret = append(ret, str)
			}
		}
	}
	return ret
}

func (obj *ClassChallengeProcessor) GenerateResult() string {
	var ret = ""
	for i := 0; i < len(obj.CaseCount); i++ {
		ret += "Case #" + strconv.Itoa(i+1) + ": " + strconv.Itoa(obj.CaseCount[i]) + "\r\n"
	}
	return ret
}

func (obj *ClassChallengeProcessor) CheckDuplicateWordInDictionaryFile() string {
	var list []string
	for i := 0; i < len(obj.DictionaryList); i++ {
		if !Contains(list, obj.DictionaryList[i]) {
			list = append(list, obj.DictionaryList[i])
		} else {
			return obj.DictionaryList[i]
		}
	}
	return ""
}

func (obj *ClassChallengeProcessor) CheckWordLengthInDictionaryFile() string {
	for i := 0; i < len(obj.DictionaryList); i++ {
		if (len(obj.DictionaryList[i]) >= 105 || len(obj.DictionaryList[i]) <= 2) && obj.DictionaryList[i] != "" {
			return obj.DictionaryList[i]
		}
	}
	return ""
}

func (obj *ClassChallengeProcessor) CheckDictionaryFileLength() bool {
	content, err := ioutil.ReadFile(obj.DictionaryFile)
	if err != nil {
		log.Fatal(err)
	}
	content = []byte(strings.ReplaceAll(string(content), "\r\n", ""))
	content = []byte(strings.ReplaceAll(string(content), " ", ""))
	if len(content) > 105 {
		return false
	}
	return true
}

func (obj *ClassChallengeProcessor) ParseDictionaryFile() {
	obj.DictionaryList = []string{}
	content, err := ioutil.ReadFile(obj.DictionaryFile)
	if err != nil {
		log.Fatal(err)
	}

	var ar = strings.Split(string(content), "\r\n")

	for i := 0; i < len(ar); i++ {
		obj.DictionaryList = append(obj.DictionaryList, ar[i])
	}
}

func (obj *ClassChallengeProcessor) ParseSearchFile() {
	obj.CaseCount = []int{}
	obj.SearchList = []string{}
	content, err := ioutil.ReadFile(obj.SearchFile)
	if err != nil {
		log.Fatal(err)
	}

	var ar = strings.Split(string(content), "\r\n")

	for i := 0; i < len(ar); i++ {
		obj.SearchList = append(obj.SearchList, ar[i])
		obj.CaseCount = append(obj.CaseCount, 0)
	}
}

func delChar(input string, index int) string {
	var s = []rune(input)
	return string(append(s[0:index], s[index+1:]...))
}

func Contains(a []string, x string) bool {
	for _, n := range a {
		if x == n {
			return true
		}
	}
	return false
}

var dictionary = "Dictionary.txt"
var search = "Input.txt"
var processor ClassChallengeProcessor

func main() {
	processor = ClassChallengeProcessor{
		DictionaryFile: dictionary,
		SearchFile:     search}
	analyze(processor)
}

func analyze(obj ClassChallengeProcessor) {
	if isValidInputs() {
		obj.Process()
		fmt.Println(obj.GenerateResult())
	}
}

func isValidInputs() bool {
	if dictionary == "" {
		fmt.Println("Please browse for the directory file.")
		return false
	}

	if search == "" {
		fmt.Println("Please browse for the search file.")
		return false
	}

	_, err := ioutil.ReadFile(dictionary)
	if err != nil {
		fmt.Println("Invalid Directory File")
		return false
	}

	_, err2 := ioutil.ReadFile(search)
	if err2 != nil {
		fmt.Println("Invalid Search File")
		return false
	}

	var msg = processor.CheckDuplicateWordInDictionaryFile()
	if msg != "" {
		fmt.Println("Duplicate word found: " + msg)
		fmt.Println("No two words in the dictionary should be the same." + msg)
		return false
	}

	msg = processor.CheckWordLengthInDictionaryFile()
	if msg != "" {
		if len(msg) <= 2 {
			fmt.Println("This word is too short: " + msg)
		} else if len(msg) >= 105 {
			fmt.Println("This word is too long: " + msg)
		}
		fmt.Println("Each word in the dictionary must be between 2 and 105 letters long.")
		return false
	}

	if !processor.CheckDictionaryFileLength() {
		fmt.Println("The sum of lengths of all words in the dictionary must not exceed 105.")
		return false
	}

	return true
}
