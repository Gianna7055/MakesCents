import { StyleSheet } from "react-native";


export const globalStyles = StyleSheet.create({
    Screen: {
        backgroundColor: '#DDF0E5',
        fontFamily: 'Inter',
        flex: 1
    },
    ButtonContainer: {
        paddingTop: 15,
        alignItems: 'center'
    },
    ButtonWrapper: {
        backgroundColor: '#304382',
        borderRadius: 10,
        justifyContent: 'center', 
        alignItems: 'center',
        height: 50,
        paddingHorizontal: 15
    },
    Button: {
        fontSize: 15,
        fontWeight: 'bold',
        color: '#FFF',
    },
});
