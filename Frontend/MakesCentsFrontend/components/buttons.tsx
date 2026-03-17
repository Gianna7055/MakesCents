import React from 'react'
import {View, Text, TouchableOpacity} from 'react-native'
import { globalStyles } from '../css/styles';

type ButtonProps = {
    name: string;
    onPress: () => void;
    style?: any;
    textStyle?: any;
    containerStyle?: any;
};

const Button = ({ name, onPress, style, textStyle, containerStyle }: ButtonProps) => {
    return(
        <View style={[globalStyles.ButtonContainer, containerStyle]}>
            <TouchableOpacity onPress={onPress}>
                <View style={[globalStyles.ButtonWrapper, style]}>
                    <Text style={[globalStyles.Button, textStyle]}>{name}</Text>
                </View>
            </TouchableOpacity>
        </View>
    );
};

export { Button };